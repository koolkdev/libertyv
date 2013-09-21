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

namespace LibertyV.RPF7.Entries
{
    public class ResourceEntry : FileEntry
    {
        public uint SystemFlag;
        public uint GraphicsFlag;
        public int Type 
        {
            set
            {
                SystemFlag &= 0x0fffffff;
                SystemFlag |= ((uint)value & 0xf0) << 24;
                GraphicsFlag &= 0x0fffffff;
                GraphicsFlag |= ((uint)value & 0xf) << 28;
            }
            get {
                return GetResourceTypeFromFlags(this.SystemFlag, this.GraphicsFlag);
            }
        }

        public int SystemSize
        {
            get
            {
                return GetSizeFromSystemFlag(this.SystemFlag);
            }
        }
        public int GraphicSize
        {
            get
            {
                return GetSizeFromGraphicsFlag(this.GraphicsFlag);
            }
        }

        public ResourceEntry(String filename, IStreamCreator data, uint systemFlag, uint graphicsFlag)
            : base(filename, data)
        {
            this.SystemFlag = systemFlag;
            this.GraphicsFlag = graphicsFlag;
        }

        public override void Export(String foldername)
        {
            // TODO: Multiplie option on how to extract
            Stream stream = this.Data.GetStream();
            if (this.SystemSize != 0)
            {
                using (FileStream file = File.OpenWrite(Path.Combine(foldername, this.Name + ".sys")))
                {
                    stream.CopyTo(file, this.SystemSize);
                }
            }

            if (this.GraphicSize != 0)
            {
                using (FileStream file = File.OpenWrite(Path.Combine(foldername, this.Name + ".gfx")))
                {
                    stream.CopyTo(file, this.GraphicSize);
                }
            }
        }

        static private int Reverse4Bits(int num)
        {
            return (num >> 3) | ((num >> 1) & 2) | ((num & 2) << 1) | ((num & 1) << 3);
        }

        static public int GetSizeFromFlag(uint flag, int baseSize) 
        {
            baseSize <<= (int)(flag & 0xf);
            int size = (int)((((flag >> 17) & 0x7f) + (((flag >> 11) & 0x3f) << 1) + (((flag >> 7) & 0xf) << 2) + (((flag >> 5) & 0x3) << 3) + (((flag >> 4) & 0x1) << 4)) * baseSize);
            for (int i = 0; i < 4; ++i) {
                size += (((flag >> (24 + i)) & 1) == 1) ? (baseSize >> (1 + i)) : 0;
            }
            return  size;
        }

        static public int GetSizeFromSystemFlag(uint flag)
        {
            if (GlobalOptions.Platform == GlobalOptions.PlatformType.PLAYSTATION3)
            {
                return GetSizeFromFlag(flag, 0x1000);
            }
            else
            { // XBOX 360
                return GetSizeFromFlag(flag, 0x2000);
            }
        }

        static public int GetSizeFromGraphicsFlag(uint flag)
        {
            if (GlobalOptions.Platform == GlobalOptions.PlatformType.PLAYSTATION3)
            {
                return GetSizeFromFlag(flag, 0x1580);
            }
            else
            { // XBOX 360
                return GetSizeFromFlag(flag, 0x2000);
            }
        }
        
        static public int GetResourceTypeFromFlags(uint systemFlag, uint graphicsFlag) 
        {
            return (int)(((graphicsFlag >> 28) & 0xF) | (((systemFlag >> 28) & 0xF) << 4));
        }

        static public bool IsResourceEncrypted(int resourceType)
        {
            // Is xsc is the only encryped resource? Is there a better way to deremine whether it is encrypted or not?
            return resourceType == 0x9;
        }
    }
}
