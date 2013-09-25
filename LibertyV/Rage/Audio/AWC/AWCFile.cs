using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibertyV.Utils;

namespace LibertyV.Rage.Audio.AWC
{
    class AWCFile : IDisposable
    {
        Stream Stream;
        bool MultiChannel = false;

        List<IAudio> AudioStreams = new List<IAudio>();
        List<int> AudioIds = new List<int>();

        private byte Tag(string s)
        {
            return Tag(Hash.jenkins_one_at_a_time_hash(s));
        }

        private byte Tag(uint hash)
        {
            return (byte)(hash & 0xff);
        }

        public AWCFile(Stream stream)
        {
            this.Stream = stream;

            Structs.AWCHeader header = new Structs.AWCHeader(stream);

            if ((header.Flags >> 8) != 0xFF)
            {
                throw new Exception("Unsupported flag");
            }

            // first bit - means that there are unknown word for each stream after this header
            // second bit - I think that it means that not all the tags are in the start of the file, but all the tags of a stream are near the data tag
            // third bit - Multi channel audio
            if ((header.Flags & 0xF8) != 0)
            {
                throw new Exception("Unsupported flag");
            }

            MultiChannel = ((header.Flags & 4) == 4);

            // 0x10 - Header size
            int audioInfoStart = 0x10 + ((header.Flags & 1) == 1 ? (2 * header.StreamsCount) : 0);
            stream.Seek(audioInfoStart, SeekOrigin.Begin);
            List<Structs.StreamInfo> info = new List<Structs.StreamInfo>();
            Dictionary<int, Dictionary<byte, Structs.ChunkInfo>> streamsChunks = new Dictionary<int, Dictionary<byte, Structs.ChunkInfo>>();

            for (int i = 0; i < header.StreamsCount; ++i)
            {
                info.Add(new Structs.StreamInfo(Stream, header.BigEndian));
            }

            for (int i = 0; i < header.StreamsCount; ++i)
            {
                streamsChunks[info[i].Id] = new Dictionary<byte, Structs.ChunkInfo>();
                for (int j = 0; j < info[i].TagsCount; ++j)
                {
                    Structs.ChunkInfo chunk = new Structs.ChunkInfo(stream, header.BigEndian);
                    streamsChunks[info[i].Id][chunk.Tag] = chunk;
                }
            }

            if (MultiChannel)
            {
                List<Structs.FormatChunk> streamsInfo = new List<Structs.FormatChunk>();
                Structs.ChannelsInfoChunkHeader channelsInfoHeader;
                // Haven't figured out that hash yet
                // stream with id 0 is just info on the other channels
                using (Stream chunkReader = new ChunkStream(this.Stream, streamsChunks[0][Tag(0x81F95048)]))
                {
                    channelsInfoHeader = new Structs.ChannelsInfoChunkHeader(chunkReader, header.BigEndian);
                    if (channelsInfoHeader.ChannelsCount != header.StreamsCount - 1)
                    {
                        throw new Exception("Unexcepted number of channels");
                    }

                    for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
                    {
                        streamsInfo.Add(new Structs.FormatChunk(new Structs.ChannelsInfoChunkItem(chunkReader, header.BigEndian)));
                        AudioIds.Add(info[i + 1].Id);
                    }
                }

                AudioStreams.Add(new MultiChannelAudio(new ChunkStream(this.Stream, streamsChunks[0][Tag("data")]), channelsInfoHeader, streamsInfo, header.BigEndian));
            }
            else
            {
                List<Structs.FormatChunk> streamsInfo = new List<Structs.FormatChunk>();
                for (int i = 0; i < header.StreamsCount; ++i)
                {
                    using (Stream chunkReader = new ChunkStream(this.Stream, streamsChunks[info[i].Id][Tag("format")]))
                    {
                        AudioStreams.Add(new Audio(new ChunkStream(this.Stream, streamsChunks[info[i].Id][Tag("data")]), new Structs.FormatChunk(chunkReader, header.BigEndian)));
                        AudioIds.Add(info[i].Id);
                    }
                }
            }

            // note: there is fourth unknown 0x21E86A3 tag
        }

        public void ExportWav(string baseName)
        {
            if (MultiChannel)
            {
                for (int i = 0; i < ((MultiChannelAudio)AudioStreams[0]).Channels.Count; ++i)
                {
                    using (Stream file = File.Create(baseName + "." + AudioIds[i] + ".wav"))
                    {
                        ExportWav(((MultiChannelAudio)AudioStreams[0]).Channels[i], file);
                    }
                }
            }
            else
            {
                for (int i = 0; i < AudioStreams.Count; ++i)
                {
                    using (Stream file = File.Create(baseName + "." + AudioIds[i] + ".wav"))
                    {
                        ExportWav(AudioStreams[i], file);
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (IAudio audio in AudioStreams)
            {
                audio.Dispose();
            }

            Stream.Dispose();
        }

        public static void ExportWav(IAudio audio, Stream output)
        {
            WAVFile.WAVFromPCM(audio.GetPCMStream(), output, (short)audio.GetChannels(), audio.GetSamplesPerSecond(), audio.GetBits());
        }
    }
}
