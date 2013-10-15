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
    class SplittedAudio : IAudio
    {
        List<Stream> Streams;
        List<Tuple<int, int>> StreamsSamples;
        uint Samples;
        int SamplesPerSecond;

        public SplittedAudio(List<Stream> streams, List<Tuple<int, int>> streamsSamples, uint samples, int samplesPerSecond)
        {
            this.Samples = samples;
            this.SamplesPerSecond = samplesPerSecond;
            this.StreamsSamples = streamsSamples;
            Streams = streams;
        }

        public int GetBits()
        {
            return GlobalOptions.AudioBits;
        }

        public uint GetSamplesCount()
        {
            return Samples;
        }

        public int GetSamplesPerSecond()
        {
            return SamplesPerSecond;
        }

        public int GetChannels()
        {
            return 1;
        }

        public int GetSize()
        {
            return (int)((this.GetBits() / 8) * this.GetChannels() * this.GetSamplesCount());
        }

        public Stream GetPCMStream()
        {
            // At first I made a decoder for each stream. But the problem is that mp3 is stateful, so I did something ugly.
            SplittedAudioPCMStream stream = new SplittedAudioPCMStream(this.Streams.Select(_stream => new PartialStream(_stream, 0, _stream.Length)).ToList<Stream>(), this.StreamsSamples, this.GetChannels(), this.GetBits());
            stream.SetDecoderStream(Audio.GetCodecStream(stream.GetRawReadStream()));
            return stream;
        }

        public void Dispose()
        {
            foreach (Stream stream in Streams)
            {
                stream.Dispose();
            }
        }
    }
}
