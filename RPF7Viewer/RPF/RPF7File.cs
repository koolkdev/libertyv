/*
 
    RPF7Viewer - Viewer for RAGE Package File version 7
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
using RPF7Viewer.Utils;
using RPF7Viewer.RPF.Entries;

namespace RPF7Viewer.RPF
{
    public class RPF7File
    {
        private BitsStream Stream;

        private int flags;
        public int shiftNameAccessBy;
        private bool sixteenRoundsDecrypt;
        
        public RPF7Entry Root;
        public String Filename;

        public RPF7File(Stream inputStream, String filname = "")
        {
            this.Filename = filname;
            Stream = new BitsStream(inputStream);
            if (Stream.ReadString(4) != "RPF7")
            {
                throw new Exception("Invalid RPF Magic");
            }

            int entriesCount = Stream.ReadInt();
            if (Stream.ReadBits(1) != 1) { // unknown flag, always 1
                throw new Exception("Expected flag to be 1, check this");
            }

            shiftNameAccessBy = (int)Stream.ReadBits(3);

            int entriesNamesLength = (int)Stream.ReadBits(28);

            // unknwon flags
            flags = Stream.ReadInt();
            sixteenRoundsDecrypt = (flags >> 28) == 0xf;

            this.Stream.Seek(0x10 * (entriesCount + 1));
            BitsStream filenames = new BitsStream(new MemoryStream(this.Decrypt(this.Stream.ReadBytes(entriesNamesLength))));

            // Go back to header
            this.Stream.Seek(0x10);
            this.Root = RPF7Entry.CreateFromHeader(this.Decrypt(Stream.ReadBytes(0x10)), this, filenames);

            if (!(this.Root is RPF7DirectoryEntry))
            {
                throw new Exception("Expected root to be directory");
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if (sixteenRoundsDecrypt)
            {
                for (int i = 0; i < 16; ++i)
                {
                    AESDecryptor.Decrypt(data);
                }
                return data;
            }
            else
            {
                return AESDecryptor.Decrypt(data); ;
            }
        }

        public byte[] Decompress(byte[] data, int uncompressedSize)
        {
            // The compression algorithm is pltaform specified, so it should be here

            // Right now I support the xbox compression only
            return XCompress.Decompress(data, uncompressedSize);
        }

        public byte[] Read(long offset, int length)
        {
            this.Stream.Seek(offset);
            return this.Stream.ReadBytes(length);
        }
    }
}
