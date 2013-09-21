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
using LibertyV.Utils;

namespace LibertyV.RPF7.Entries
{
    public abstract class Entry
    {
        public String Name;
        public DirectoryEntry Parent = null;

        public Entry(String filename) {
            this.Name = filename;
        }

        public virtual void Export(String foldername)
        {
            throw new Exception("Not implemented"); 
        }
     
        public static Entry CreateFromHeader(Structs.RPF7EntryInfoTemplate info, RPF7File file, MemoryStream entriesInfo, MemoryStream filenames)
        {
            bool isResource = info.Field1 == 1;
            long offset = (long)info.Field2;
            int compressedSize = (int)info.Field3;
            int filenameOffset = (int)info.Field4;

            filenames.Seek(filenameOffset << file.Info.ShiftNameAccessBy, SeekOrigin.Begin);
            String filename = "";
            // Read null-terminated filename
            char currentChar;
            while ((currentChar = (char)filenames.ReadByte()) != 0)
            {
                filename += currentChar;
            }

            if (offset == 0x7FFFFF)
            {
                // Is a Directory
                if (isResource)
                {
                    throw new Exception("Invalid type");
                }
                int subentriesStartIndex = (int)info.Field5;
                int subentriesCount = (int)info.Field6;
                List<Entry> entries = new List<Entry>();
                for (int i = 0; i < subentriesCount; ++i)
                {
                    entriesInfo.Seek(0x10 * (i + subentriesStartIndex), SeekOrigin.Begin);
                    entries.Add(Entry.CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), file, entriesInfo, filenames));
                }
                return new DirectoryEntry(filename, entries);
            }

            offset <<= 9;

            if (isResource)
            {
                if (compressedSize == 0xFFFFFF)
                {
                    throw new Exception("Resource with size -1, not supported");
                }
                uint systemFlag = info.Field5;
                uint graphicsFlag = info.Field6;
                return new ResourceEntry(filename, new ResourceStreamCreator(file, offset, compressedSize, systemFlag, graphicsFlag), systemFlag, graphicsFlag);
            }

            // Regular file
            int uncompressedSize = (int)info.Field5;
            int isEncrypted = (int)info.Field6;

            if (compressedSize == 0)
            {
                // Uncompressed file
                if (isEncrypted != 0)
                {
                    throw new Exception("Unexcepted value");
                }
                return new RegularFileEntry(filename, new FileStreamCreator(file, offset, uncompressedSize), false);
            }
            else
            {
                // Compressed file
                return new RegularFileEntry(filename, new CompressedFileStreamCreator(file, offset, compressedSize, uncompressedSize, isEncrypted != 0), true);
            }
        }
    }
}
