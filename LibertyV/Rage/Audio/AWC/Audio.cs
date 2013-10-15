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
using LibertyV.Rage.Audio.Codecs.MP3;
using LibertyV.Utils;
using LibertyV.Rage.Audio.Codecs.XMA;

namespace LibertyV.Rage.Audio.AWC
{
    class Audio : IAudio
    {
        Stream Data;
        uint Samples;
        int SamplesPerSecond;

        public Audio(Stream data, uint samples, int samplesPerSecond)
        {
            this.Samples = samples;
            this.SamplesPerSecond = samplesPerSecond;
            this.Data = data;
        }

        public Audio(Stream data, Structs.FormatChunk chunkInfo)
        {
            this.Samples = chunkInfo.Samples;
            this.SamplesPerSecond = chunkInfo.SamplesPerSecond;
            this.Data = data;
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
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                // The playstation3 use mp3 encode
                // The partial stream is because we want that each reader will have its own seeker
                return new MP3DecoderStream(new PartialStream(this.Data, 0, this.Data.Length));
            }
            else if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                return new XMA2DecoderStream(new PartialStream(this.Data, 0, this.Data.Length));
            }
            return null;
        }

        public static Stream GetCodecStream(Stream input)
        {
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                // The playstation3 use mp3 encode
                // The partial stream is because we want that each reader will have its own seeker
                return new MP3DecoderStream(input);
            }
            else if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                return new XMA2DecoderStream(input);
            }
            return null;
        }

        public void Dispose()
        {
            Data.Dispose();
        }
    }
}
