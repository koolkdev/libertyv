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

namespace RPF7Viewer.RPF.Entries
{
    public class RPF7ResourceEntry : RPF7FileEntry
    {
        public uint SystemFlag;
        public uint GraphicsFlag;
        public ulong Type 
        {   
            get {
                return GetResourceTypeFromFlags(this.SystemFlag, this.GraphicsFlag);
            }
        }

        public int SystemSize
        {
            get
            {
                return GetSizeFromFlag(this.SystemFlag);
            }
        }
        public int GraphicSize
        {
            get
            {
                return GetSizeFromFlag(this.GraphicsFlag);
            }
        }

        public RPF7ResourceEntry(String filename, IRPFBuffer data, uint systemFlag, uint graphicsFlag)
            : base(filename, data)
        {
            this.SystemFlag = systemFlag;
            this.GraphicsFlag = graphicsFlag;
        }

        public override void Export(String foldername)
        {
            byte [] data = this.Data.GetData();

            if (this.SystemSize != 0)
            {
                byte[] sysData = new byte[this.SystemSize];
                Buffer.BlockCopy(data, 0, sysData, 0, this.SystemSize);
                File.WriteAllBytes(Path.Combine(foldername, this.Filename + ".sys"), sysData);
            }

            if (this.GraphicSize != 0)
            {
                byte[] gfxData = new byte[this.GraphicSize];
                Buffer.BlockCopy(data, this.SystemSize, gfxData, 0, this.GraphicSize);
                File.WriteAllBytes(Path.Combine(foldername, this.Filename + ".gfx"), gfxData);
            }
        }

        static private int Reverse4Bits(int num)
        {
            return (num >> 3) | ((num >> 1) & 2) | ((num & 2) << 1) | ((num & 1) << 3);
        }

        static public int GetSizeFromFlag(uint flag) 
        {
            return (int)((((flag >> 17) & 0x7f) + (((flag >> 11) & 0x3f) << 1) + (((flag >> 7) & 0xf) << 2) + (((flag >> 5) & 0x3) << 3) + (((flag >> 4) & 0x1) << 4)) << (13 + ((int)(flag & 0xf)))) + (((Reverse4Bits((int)((flag >> 24) & 0xf))) << (9 + (int)(flag & 0xf))));
        }
        
        static public uint GetResourceTypeFromFlags(uint systemFlag, uint graphicsFlag) 
        {
            return ((graphicsFlag >> 28) & 0xF) | (((systemFlag >> 28) & 0xF) << 4);
        }

        static public bool IsResourceEncrypted(uint resourceType)
        {
            // Is xsc is the only encryped resource? Is there a better way to deremine whether it is encrypted or not?
            return resourceType == 0x9;
        }
    }
}
