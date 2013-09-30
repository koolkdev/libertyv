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
        List<Tuple<int, int>> StreamsSamples;
        uint Samples;
        int SamplesPerSecond;

        public SplittedAudio(List<Stream> streams, List<Tuple<int, int>> streamsSamples, uint samples, int samplesPerSecond)
        {
            this.Samples = samples;
            this.SamplesPerSecond = samplesPerSecond;
            this.StreamsSamples = streamsSamples;
            Streams = streams.Select(stream => new Audio(stream, samples, samplesPerSecond)).ToList();
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

        public Stream GetPCMStream()
        {
            return new SplittedAudioPCMStream(Streams.Select(stream => stream.GetPCMStream()).ToList(), StreamsSamples, this.GetChannels(), this.GetBits());
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
