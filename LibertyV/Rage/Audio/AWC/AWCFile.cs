/*
 
    LibertyV - Viewer/Editor for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

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
                List<Structs.ChannelsInfoChunkItem> streamsInfo = new List<Structs.ChannelsInfoChunkItem>();
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
                        streamsInfo.Add(new Structs.ChannelsInfoChunkItem(chunkReader, header.BigEndian));
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


        public void ExportWav(string baseName, IProgressReport progress = null)
        {
            int bytesWrote = 0;
            if (MultiChannel)
            {
                /*using (Stream file = File.Create(baseName + ".wav"))
                {
                    ExportWav(AudioStreams[0], file);
                }*/
                if (progress != null)
                {
                    // Calculate how many bytes are going to be written
                    progress = new SubProgressReport(progress, (((MultiChannelAudio)AudioStreams[0]).Channels.Sum(audio => audio.GetSize())));
                }
                for (int i = 0; i < ((MultiChannelAudio)AudioStreams[0]).Channels.Count; ++i)
                {
                    using (Stream file = File.Create(baseName + "." + AudioIds[i] + ".wav"))
                    {
                        if (progress != null)
                        {
                            progress.SetMessage("Decoding " + AudioIds[i]);
                        }
                        int audioSize = ((MultiChannelAudio)AudioStreams[0]).Channels[i].GetSize() ;
                        try
                        {
                            ExportWav(((MultiChannelAudio)AudioStreams[0]).Channels[i], file, progress == null ? null : new SubProgressReport(progress, bytesWrote, audioSize));
                        }
                        catch (Exception e)
                        {
                            file.Close();
                            // Delete uncompleted file
                            File.Delete(baseName + "." + AudioIds[i] + ".wav");
                            throw e;
                        }
                        bytesWrote += audioSize;
                    }
                }
            }
            else
            {
                for (int i = 0; i < AudioStreams.Count; ++i)
                {
                    if (progress != null)
                    {
                        // Calculate how many bytes are going to be written
                        progress = new SubProgressReport(progress, AudioStreams.Sum(audio => audio.GetSize()));
                    }
                    using (Stream file = File.Create(baseName + "." + AudioIds[i] + ".wav"))
                    {
                        if (progress != null)
                        {
                            progress.SetMessage("Decoding " + AudioIds[i]);
                        }
                        int audioSize = AudioStreams[i].GetSize();
                        try
                        {
                            ExportWav(AudioStreams[i], file, progress == null ? null : new SubProgressReport(progress, bytesWrote, audioSize));
                        }
                        catch (OperationCanceledException e)
                        {
                            file.Close();
                            // Delete uncompleted file
                            File.Delete(baseName + "." + AudioIds[i] + ".wav");
                            throw e;
                        }
                        bytesWrote += audioSize;
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

        private static void ExportWav(IAudio audio, Stream output, IProgressReport writingProgress = null)
        {
            using (Stream input = audio.GetPCMStream())
            {
                WAVFile.WAVFromPCM(input, output, (short)audio.GetChannels(), audio.GetSamplesPerSecond(), audio.GetBits(), (int)audio.GetSamplesCount(), writingProgress);
            }
        }
    }
}
