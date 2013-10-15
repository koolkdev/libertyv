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
    static class WAVFile
    {
        public static void WAVFromPCM(Stream input, Stream output, short channels, int samplesPerSec, int bitsPerSample, int samples = 0, IProgressReport writingProgress = null)
        {
            short sample_size = (short)((bitsPerSample / 8) * channels);
            
            using (BinaryWriter writer = new BinaryWriter(new StreamKeeper(output)))
            {
                writer.Write(new char[] { 'R', 'I', 'F', 'F' });
                writer.Write((int)0); // Skip size of wave file
                writer.Write(new char[] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[] { 'f', 'm', 't', ' ' });
                writer.Write((int)16); // Size of header
                writer.Write((short)1); // Format tag - PCM
                writer.Write(channels);
                writer.Write(samplesPerSec);
                writer.Write((int)(sample_size * samplesPerSec)); // average bytes per sec
                writer.Write(sample_size); // full sample size..
                writer.Write((short)bitsPerSample);
                writer.Write(new char[] { 'd', 'a', 't', 'a' });
                writer.Write((int)0); // Skip size of data
            }
            if (samples != 0)
            {
                if (input.CopyToCount(output, samples * sample_size, writingProgress) != samples * sample_size)
                {
                    // Check output size
                    throw new Exception("Invalid WAV size");
                }
            }
            else
            {
                // Write the pcm
                input.CopyTo(output);

                if (writingProgress != null)
                {
                    writingProgress.SetProgress(writingProgress.GetFullValue());
                }
            
            }
            output.Seek(4, SeekOrigin.Begin);
            // Write the size
            using (BinaryWriter writer = new BinaryWriter(new StreamKeeper(output)))
            {
                writer.Write((int)(output.Length - 8));
            }
            output.Seek(40, SeekOrigin.Begin);
            // Write the size
            using (BinaryWriter writer = new BinaryWriter(new StreamKeeper(output)))
            {
                writer.Write((int)(output.Length - 44));
            }
        }
    }
}
