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

namespace LibertyV.RPF.V7.Entries
{
    public class RegularFileEntry : FileEntry
    {
        public bool Compressed;

        public RegularFileEntry(String filename, IStreamCreator data, bool compressed)
            : base(filename, data)
        {
            this.Compressed = compressed;
        }

        public override void Write(Stream stream)
        {
            // optimization: Check if we have the data from the original file, and we don't need to encrypt and compress it again
            if (this.Compressed && this.Data.GetType() == typeof(CompressedFileStreamCreator))
            {
                (this.Data as CompressedFileStreamCreator).WriteRaw(stream);
            }
            else if (!this.Compressed && this.Data.GetType() == typeof(FileStreamCreator))
            {
                (this.Data as FileStreamCreator).WriteRaw(stream);
            }
            else
            {
                // we need to create it..
                Stream baseStream = new StreamKeeper(stream);
                using (Stream input = this.Data.GetStream())
                {
                    using (Stream output = this.Compressed ? Platform.GetCompressStream(AES.EncryptStream(baseStream)) : baseStream)
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }
    }
}
