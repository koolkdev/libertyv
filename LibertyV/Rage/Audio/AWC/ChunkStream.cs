using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibertyV.Utils;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class ChunkStream : PartialStream
    {
        public ChunkStream(Stream stream, Structs.ChunkInfo chunk)
            : base(stream, chunk.Offset, chunk.Size)
        {
        }
    }
}
