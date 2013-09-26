using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class SplittedAudio : IAudio
    {
        List<Audio> Streams;
        uint Samples;
        int SamplesPerSecond;

        public SplittedAudio(List<Stream> streams, uint samples, int samplesPerSecond)
        {
            this.Samples = samples;
            this.SamplesPerSecond = samplesPerSecond;
            Streams = streams.Select(stream => new Audio(stream, samples, samplesPerSecond)).ToList();
        }

        public int GetBits()
        {
            // I export all in 24bit right now
            return 24;
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

        public Stream GetPCMStream()
        {
            return new SplittedAudioPCMStream(Streams.Select(stream => stream.GetPCMStream()).ToList());
        }

        public void Dispose()
        {
            foreach (IAudio stream in Streams)
            {
                stream.Dispose();
            }
        }
    }
}
