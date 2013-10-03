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
