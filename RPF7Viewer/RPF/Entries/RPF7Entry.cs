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

namespace RPF7Viewer.RPF.Entries
{
    public class RPF7Entry
    {
        public String Filename;
        public RPF7Entry Parent = null;

        public RPF7Entry(String filename) {
            this.Filename = filename;
        }

        public virtual void Export(String foldername)
        {
            throw new Exception("Not implemented"); 
        }
     
        public static RPF7Entry CreateFromHeader(byte[] data, RPF7File file, BitsStream filenames)
        {
            if (data.Length != 0x10)
            {
                throw new Exception("Invalid header length.");
            }

            BitsStream stream = new BitsStream(new MemoryStream(data));

            bool isResource = stream.ReadBool();
            int offset = (int)stream.ReadBits(23);
            int compressedSize = (int)stream.ReadBits(24);
            int filenameOffset = (int)stream.ReadBits(16);

            filenames.Seek(filenameOffset << file.shiftNameAccessBy);
            String filename = filenames.ReadCString();

            if (offset == 0x7FFFFF)
            {
                // Is a Directory
                if (isResource)
                {
                    throw new Exception("Invalid type");
                }
                int subentriesStartIndex = stream.ReadInt();
                int subentriesCount = stream.ReadInt();
                List<RPF7Entry> entries = new List<RPF7Entry>();
                for (int i = 0; i < subentriesCount; ++i)
                {
                    entries.Add(RPF7Entry.CreateFromHeader(file.Decrypt(file.Read(0x10 * (i + subentriesStartIndex + 1), 0x10)), file, filenames));
                }
                return new RPF7DirectoryEntry(filename, entries);
            }

            offset <<= 9;

            if (isResource)
            {
                if (compressedSize == 0xFFFFFF)
                {
                    throw new Exception("Resource with size -1, not supported");
                }
                uint systemFlag = (uint)stream.ReadInt();
                uint graphicsFlag = (uint)stream.ReadInt();
                return new RPF7ResourceEntry(filename, new RPF7ResourceFileBuffer(file, offset, compressedSize, systemFlag, graphicsFlag), systemFlag, graphicsFlag);
            }

            // Regular file
            int uncompressedSize = stream.ReadInt();
            int isEncrypted = stream.ReadInt();

            if (compressedSize == 0)
            {
                // Uncompressed file
                if (isEncrypted != 0)
                {
                    throw new Exception("Unexcepted value");
                }
                return new RPF7RegularFileEntry(filename, new RPF7FileBuffer(file, offset, uncompressedSize), false);
            }
            else
            {
                // Compressed file
                return new RPF7RegularFileEntry(filename, new RPF7CompressedFileBuffer(file, offset, compressedSize, uncompressedSize, isEncrypted != 0), true);
            }
        }
    }
}
