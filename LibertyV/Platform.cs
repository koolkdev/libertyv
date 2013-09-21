using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using LibertyV.Utils;

namespace LibertyV
{
    static class Platform
    {
        public enum PlatformType
        {
            NONE,
            XBOX360,
            PLAYSTATION3
        };

        public static Stream GetCompressStream(Stream stream)
        {
            if (GlobalOptions.Platform == PlatformType.PLAYSTATION3)
            {
                return new DeflateStream(stream, CompressionMode.Compress);
            }
            else
            {
                return new XMemCompressStream(stream);
            }
        }

        public static Stream GetDecompressStream(Stream stream)
        {
            if (GlobalOptions.Platform == PlatformType.PLAYSTATION3)
            {
                return new DeflateStream(stream, CompressionMode.Decompress);
            }
            else
            {
                return new XMemDecompressStream(stream);
            }
        }
    }
}
