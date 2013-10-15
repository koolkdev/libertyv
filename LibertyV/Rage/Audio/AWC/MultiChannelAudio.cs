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
    class MultiChannelAudio : IAudio
    {
        public List<SplittedAudio> Channels = new List<SplittedAudio>();

        public MultiChannelAudio(Stream data, Structs.ChannelsInfoChunkHeader channelsInfoHeader, List<Structs.ChannelsInfoChunkItem> channelsInfo, bool bigEndian)
        {
            int chunkSize = 0x800;

            List<Stream>[] channelsStreams = new List<Stream>[channelsInfoHeader.ChannelsCount];
            List<Tuple<int, int>>[] samples = new List<Tuple<int, int>>[channelsInfoHeader.ChannelsCount];

            for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
            {
                channelsStreams[i] = new List<Stream>();
                samples[i] = new List<Tuple<int, int>>();
            }

            while (data.Position != data.Length)
            {
                int totalChunks = 0;
                long startPos = data.Position;
                long pos = startPos;
                int[] dataSizes = new int[channelsInfoHeader.ChannelsCount];
                int[] firstNewData = new int[channelsInfoHeader.ChannelsCount];
                for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
                {
                    Structs.ChannelChunkHeader header = new Structs.ChannelChunkHeader(data, bigEndian);
                    dataSizes[i] = header.DataSize;
                    totalChunks += header.Chunks;

                    samples[i].Add(Tuple.Create(header.SamplesSkip, (int)header.Samples - header.SamplesSkip));
                }

                int headerSize = totalChunks * 4 + channelsInfoHeader.ChannelsCount * Structs.ChannelChunkHeader.Size;
                headerSize += (((-headerSize) % chunkSize) + chunkSize) % chunkSize;
                
                pos += headerSize;
                for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
                {
                    channelsStreams[i].Add(new PartialStream(data, pos, dataSizes[i]));

                    if (channelsInfo[i].RoundSize != 0)
                    {
                        dataSizes[i] += (((-dataSizes[i]) % channelsInfo[i].RoundSize) + channelsInfo[i].RoundSize) % channelsInfo[i].RoundSize;
                    }
                    pos += dataSizes[i];
                }

                if (pos - startPos > channelsInfoHeader.BigChunkSize)
                {
                    throw new Exception("Chunks too big");
                }

                if (totalChunks == 0 || startPos + channelsInfoHeader.BigChunkSize > data.Length)
                {
                    throw new Exception("Unexpected value");
                }

                // After each chunk, there is header's size zeros block
                data.Seek(startPos + channelsInfoHeader.BigChunkSize, SeekOrigin.Begin);
            }

            for (int i = 0; i < channelsInfoHeader.ChannelsCount; ++i)
            {
                Channels.Add(new SplittedAudio(channelsStreams[i], samples[i], channelsInfo[i].Samples, channelsInfo[i].SamplesPerSecond));
            }

            // following the headers, there is a seek table: For each chunk, there is the dword - which is the first sample in the block
        }

        // 1. it is the same for all the channels
        // 2. we have at least one channel
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

        public int GetSize()
        {
            return (int)((this.GetBits() / 8) * this.GetChannels() * this.GetSamplesCount());
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
