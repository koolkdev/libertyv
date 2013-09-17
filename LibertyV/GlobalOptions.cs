using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibertyV
{
    static class GlobalOptions
    {
        public enum PlatformType {
            NONE,
            XBOX360,
            PLAYSTATION3
        };
        static public PlatformType Platform = PlatformType.NONE;

    }
}
