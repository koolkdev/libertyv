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
        public static void WAVFromPCM(Stream input, Stream output, short channels, int samplesPerSec, int bitsPerSample, int samples = 0)
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
                if (input.CopyToCount(output, samples * sample_size) != samples * sample_size)
                {
                    // Check output size
                    throw new Exception("Invalid WAV size");
                }
            }
            else
            {
                // Write the pcm
                input.CopyTo(output);
            
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
