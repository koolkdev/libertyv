using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class SplittedAudio : Audio
    {
        public SplittedAudio(List<Stream> streams, uint samples, int samplesPerSecond)
            : base(new SplittedStream(streams), samples, samplesPerSecond)
        {
        }
    }
}
