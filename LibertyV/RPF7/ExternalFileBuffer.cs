using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.RPF7
{
    public class ExternalFileBuffer : IBuffer
    {
        private Stream Stream;
        public ExternalFileBuffer(Stream stream )
        {
            this.Stream = stream;
        }

        public virtual byte[] GetData()
        {
            byte[] data = new byte[this.Stream.Length];

            this.Stream.Seek(0, SeekOrigin.Begin);
            if (this.Stream.Read(data, 0, (int)this.Stream.Length) != this.Stream.Length)
            {
                throw new Exception("Failed to read from stream.");
            }

            return data;            
        }

        public virtual int GetSize()
        {
            return (int)this.Stream.Length;
        }
    }
}
