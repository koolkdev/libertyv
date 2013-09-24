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
using LibertyV.Rage.RPF.V7.Entries;
using System.IO;

namespace LibertyV.Rage.RPF
{
    public class ResourceStreamCreator : CompressedFileStreamCreator
    {

        // Ignore first 0x10, it is supposed to be the header, but is it junk since we got all the information that we need about the resource from the flags?
        public ResourceStreamCreator(Stream fileStream, long offset, int compressedSize, uint systemFlag, uint graphicsFlag, string resourceType)
            : base(fileStream, offset + 0x10, compressedSize - 0x10, (int)(ResourceEntry.GetSizeFromSystemFlag(systemFlag) + ResourceEntry.GetSizeFromGraphicsFlag(graphicsFlag)), ResourceEntry.IsResourceEncrypted(resourceType))
        {
        }

    }
}
