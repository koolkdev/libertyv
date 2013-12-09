/*
 
    LibertyV - Viewer/Editor for RAGE Package File version 7
    Copyright (C) 2013  koolk <koolkdev at gmail.com>
   
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
  
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
   
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibertyV.Rage.Resources.Types;

namespace LibertyV.Rage.Resources
{
    public class ResourceReader : IDisposable
    {
        private Stream Stream = null;
        public int Offset
        {
            get
            {
                return _offset;
            }
        }

        private int _offset;
        
        private bool BigEndian;

        private ResourceReader OriginalReader;

        public ResourceReader(Stream stream, int offset = 0, ResourceReader originalReader = null)
        {
            if (originalReader == null)
            {
                this.OriginalReader = this;
            }
            else
            {
                this.OriginalReader = originalReader;
            }
            this.Stream = stream;
            this._offset = offset;
            // TODO: When we will have PC
            //this.BigEndian = GlobalOptions.Platform != Platform.PlatformType.PC;
            this.BigEndian = true;
        }

        private ulong ReadInteger(int length)
        {
            ulong result = 0;
            int shift = 0;
            while (length-- > 0)
            {
                result <<= this.BigEndian ? 8 : 0;
                result |= (ulong)this.ReadByte() << shift;
                shift += this.BigEndian ? 0 : 8;
            }
            return result;
        }

        public UInt32 ReadUInt32()
        {
            return (UInt32)ReadInteger(4);
        }

        public UInt16 ReadUInt16()
        {
            return (UInt16)ReadInteger(2);
        }

        public byte ReadByte()
        {
            lock(Stream) {
                Stream.Seek(this._offset++, SeekOrigin.Begin);
                int res = Stream.ReadByte();
                if (res == -1)
                {
                    throw new EndOfStreamException();
                }
                return (byte)res;
            }
        }

        public ResourceReader DereferencePointer()
        {
            uint ptr = this.ReadUInt32();
            if (ptr == 0)
            {
                return null;
            }
            
            if ((ptr >> 24) != 0x50)
            {
                // TODO: Resource parsing exception
                throw new Exception("Invalid resource: Invalid pointer");
            }

            ptr &= 0xFFFFFF;
            if (ptr > this.Stream.Length)
            {
                throw new Exception("Invalid resource: Invalid pointer");
            }

            return new ResourceReader(this.Stream, (int)ptr,  this.OriginalReader);
        }

        public ResourceReader Clone()
        {
            return new ResourceReader(this.Stream, this._offset, this.OriginalReader);
        }

        public void Dispose()
        {
            Close();
        }
        public void Close()
        {
            if (this == this.OriginalReader && this.Stream != null)
            {
                this.Stream.Close();
            }
            this.Stream = null;
        }
    }
}
