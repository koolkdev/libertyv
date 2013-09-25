using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    interface IAudio : IDisposable
    {
        int GetBits();
        uint GetSamplesCount();
        int GetSamplesPerSecond();
        int GetChannels();

        Stream GetPCMStream();
    }
}
