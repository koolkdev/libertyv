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
using LibertyV.Utils;
using System.IO;

namespace LibertyV.RPF7
{
    public class CompressedFileStreamCreator : FileStreamCreator
    {
        private int UncompressedSize;
        private bool Encrypted;

        public CompressedFileStreamCreator(RPF7File file, long offset, int compressedSize, int uncompressedSize, bool encrypted = true)
            : base(file, offset, compressedSize)
        {
            this.UncompressedSize = uncompressedSize;
            this.Encrypted = encrypted;
        }

        public override Stream GetStream()
        {
            if (this.Encrypted) {
                return this.File.GetDecompressStream(this.File.GetDecryptStream(base.GetStream()));
            } else {
                return this.File.GetDecompressStream(base.GetStream());
            }
        }

        public override int GetSize()
        {
            return this.UncompressedSize;
        }
    }
}
