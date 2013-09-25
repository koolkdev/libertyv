using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibertyV.Utils;

namespace LibertyV.Rage.Audio.AWC
{
    class MultiChannelAudio : IAudio
    {
        // To improve: right now I decode the same mp3 as number of channels, it can be improved
        public List<SplittedAudio> Channels = new List<SplittedAudio>();

        public MultiChannelAudio(Stream data, Structs.ChannelsInfoChunkHeader channelsInfoHeader, List<Structs.FormatChunk> channelsInfo, bool bigEndian)
        {
            int chunkSize = 0x800;
            
            List<Tuple<int, int>>[] channelsChunksSamples = new List<Tuple<int, int>>[channelsInfoHeader.ChannelsCount];
            List<Tuple<int, int>> samplesInfo = new List<Tuple<int, int>>();
            List<IAudio> audio = new List<IAudio>();
            for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
            {
                channelsChunksSamples[i] = new List<Tuple<int, int>>();
            }

            while (data.Position != data.Length)
            {
                int totalChunks = 0;
                uint samplePos = 0;
                // Warning! We assume that the header size is smaller than 0x800, but it seems to be always the case
                long startPos = data.Position;
                long pos = data.Position + chunkSize;
                for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
                {
                    Structs.ChannelChunkHeader header = new Structs.ChannelChunkHeader(data, bigEndian);
                    // number of chunk's chunks
                    if (header.Chunks != 0)
                    {
                        channelsChunksSamples[i].Add(Tuple.Create((int)samplePos, (int)header.Samples));
                        samplePos += header.Samples;
                    }
                    totalChunks += header.Chunks;
                    pos += header.Chunks * chunkSize;
                }

                if (totalChunks * 4 + channelsInfoHeader.ChannelsCount * Structs.ChannelChunkHeader.Size > chunkSize)
                {
                    // Checking our assumpetion 
                    throw new Exception("Too big chunk header");
                }

                if (pos - startPos > channelsInfoHeader.BigChunkSize)
                {
                    throw new Exception("Chunks too big");
                }

                if (totalChunks == 0 || startPos + channelsInfoHeader.BigChunkSize > data.Length)
                {
                    throw new Exception("Unexpected value");
                }

                
                //samplesInfo.Add(Tuple.Create(0, (int)samplePos));
                // We assume two things:
                // 1. it is the same for all the channels
                // 2. we have at least one channel
                audio.Add(new Audio(new PartialStream(data, startPos + chunkSize, channelsInfoHeader.BigChunkSize - chunkSize), samplePos, channelsInfo[0].SamplesPerSecond));

                // After each chunk, there is header's size zeros block
                data.Seek(startPos + channelsInfoHeader.BigChunkSize, SeekOrigin.Begin);
            }

            for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
            {
                Channels.Add(new SplittedAudio(audio, channelsChunksSamples[i], channelsInfo[i].Samples, channelsInfo[i].SamplesPerSecond));
            }
            // notice that the samples count is as the audio for all the channels is mono, which this is the case, we need to create the channels from it
            //Audio = new SplittedAudio(audio, samplesInfo, (uint)channelsInfo.Sum(info => info.Samples), channelsInfo[0].SamplesPerSecond);

            // Big question:
            // There is a waste of samples in each level, there is more information than what we need
            // What I do right now, is to cut the left samples at the end of each part (how do I know that the samples in each part are splitted perfectly for each channel?)
            // Cut the left samples from the end
            // OK, so it wrongs righ now, I need to understand how to split it. Maybe in frame start/end?
        }

        public int GetBits()
        {
            return Channels[0].GetBits();
        }

        public uint GetSamplesCount()
        {
            return Channels[0].GetSamplesCount();
        }

        public int GetSamplesPerSecond()
        {
            return Channels[0].GetSamplesPerSecond();
        }

        public int GetChannels()
        {
            return Channels.Count;
        }

        public Stream GetPCMStream()
        {
            return new MultiChannelPCMStream(Channels.Select(channel => channel.GetPCMStream()).ToList(), this.GetSamplesCount(), this.GetBits());
        }

        public void Dispose()
        {
            foreach (SplittedAudio audio in Channels)
            {
                audio.Dispose();
            }
        }
    }
}
