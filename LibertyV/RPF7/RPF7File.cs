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
using LibertyV.RPF7.Entries;
using System.IO.Compression;

namespace LibertyV.RPF7
{
    public class RPF7File
    {
        public Stream Stream;

        public Structs.RPF7Header Info;
        private bool sixteenRoundsDecrypt;
        
        public Entry Root;
        public String Filename;

        public RPF7File(Stream inputStream, String filname = "")
        {
            this.Filename = filname;
            Stream = inputStream;

            Info = new Structs.RPF7Header(Stream);

            if (new string(Info.Magic) != "RPF7")
            {
                throw new Exception("Invalid RPF Magic");
            }

            sixteenRoundsDecrypt = (Info.Flag >> 28) == 0xf;

            using (Stream decryptStream = this.GetDecryptStream(new StreamKeeper(this.Stream)))
            {
                using (BinaryReader binaryStream = new BinaryReader(decryptStream))
                {
                    MemoryStream entriesInfo = new MemoryStream(binaryStream.ReadBytes(0x10 * Info.EntriesCount));
                    MemoryStream filenames = new MemoryStream(binaryStream.ReadBytes(Info.EntriesNamesLength));
                    this.Root = Entry.CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), this, entriesInfo, filenames);
                }
            }


            if (!(this.Root is DirectoryEntry))
            {
                throw new Exception("Expected root to be directory");
            }
        }

        public Stream GetDecryptStream(Stream stream)
        {
            return AES.DecryptStream(stream, sixteenRoundsDecrypt);
        }

        public Stream GetDecompressStream(Stream stream)
        {
            // The compression algorithm is pltaform specified, so it should be here
            if (GlobalOptions.Platform == GlobalOptions.PlatformType.PLAYSTATION3)
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
