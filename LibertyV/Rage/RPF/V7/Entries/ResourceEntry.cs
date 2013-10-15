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

namespace LibertyV.Rage.RPF.V7.Entries
{
    public class ResourceEntry : FileEntry
    {
        public uint SystemFlag;
        public uint GraphicsFlag;
        public int Version
        {
            set
            {
                SystemFlag &= 0x0fffffff;
                SystemFlag |= ((uint)value & 0xf0) << 24;
                GraphicsFlag &= 0x0fffffff;
                GraphicsFlag |= ((uint)value & 0xf) << 28;
            }
            get
            {
                return GetResourceVersionFromFlags(this.SystemFlag, this.GraphicsFlag);
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

        public string ResourceType;

        // x - xenon (xbox360)
        // c - cell (ps3)
        // w - windows (pc)
        public char ResourcePlatform;

        public ResourceEntry(String filename, IStreamCreator data, uint systemFlag, uint graphicsFlag)
            : base(filename, data)
        {
            this.SystemFlag = systemFlag;
            this.GraphicsFlag = graphicsFlag;
            ResourcePlatform = Path.GetExtension(filename)[1];
            ResourceType = Path.GetExtension(filename).Substring(2);
        }

        public override FileStreamCreator TryGetOriginalFileStreamCreator()
        {
            if (this.Data.GetType() == typeof(ResourceStreamCreator) && ((ResourceStreamCreator)this.Data).Encrypted == IsResourceEncrypted(this.ResourceType))
            {
                return (this.Data as ResourceStreamCreator);
            }
            return null;
        }

        public override void Write(Stream stream)
        {
            // 0x10 ignored bytes
            stream.Write(new byte[0x10], 0, 0x10);
            FileStreamCreator originalStream = this.TryGetOriginalFileStreamCreator();
            // optimization: Check if we have the data from the original file, and we don't need to encrypt and compress it again
            if (originalStream != null)
            {
                originalStream.WriteRaw(stream);
            }
            else
            {
                // we need to create it..
                Stream baseStream = new StreamKeeper(stream);
                using (Stream input = this.Data.GetStream())
                {
                    using (Stream output = IsResourceEncrypted(this.ResourceType) ? Platform.GetCompressStream(AES.EncryptStream(baseStream)) : Platform.GetCompressStream(baseStream))
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }

        public override void Export(String foldername, IProgressReport progress = null)
        {
            string filename;
            if (Directory.Exists(foldername))
            {
                filename = Path.Combine(foldername, this.Name);
            }
            else
            {
                filename = foldername;
            }

            if (progress != null)
            {
                progress.SetMessage("Extracting " + this.Name + ".");
            }

            switch (Properties.Settings.Default.ExportResourcesChoice)
            {
                case Settings.ExportResourcesChoice.RSC7:
                    {
                        using (FileStream file = File.Create(filename))
                        {
                            using (BinaryWriter writer = new BinaryWriter(new StreamKeeper(file)))
                            {
                                // big endian resource 7 for now
                                writer.Write("RSC7".ToArray());
                                writer.Write(Structs.SwapEndian((uint)this.Version));
                                writer.Write(Structs.SwapEndian(this.SystemFlag));
                                writer.Write(Structs.SwapEndian(this.GraphicsFlag));
                            }

                            using (Stream stream = this.Data.GetStream())
                            {
                                stream.CopyToCount(file, this.Data.GetSize(), progress);
                            }
                        }
                        break;
                    }
                case Settings.ExportResourcesChoice.SYSGFX:
                    {
                        if (progress != null)
                        {
                            progress = new SubProgressReport(progress, (int)(this.SystemSize + this.GraphicsFlag));
                        }
                        using (Stream stream = this.Data.GetStream())
                        {
                            // If there is no graphics information, no need to extract into two files
                            string sysExtension = this.GraphicSize == 0 ? "" : ".sys";
                            if (this.SystemSize != 0)
                            {
                                using (FileStream file = File.Create(filename + sysExtension))
                                {
                                    stream.CopyToCount(file, this.SystemSize, progress == null ? null : new SubProgressReport(progress, 0, this.SystemSize));
                                }
                            }

                            if (this.GraphicSize != 0)
                            {
                                using (FileStream file = File.Create(filename + ".gfx"))
                                {
                                    stream.CopyToCount(file, this.GraphicSize, progress == null ? null : new SubProgressReport(progress, this.SystemSize, (int)(this.SystemSize + this.GraphicsFlag)));
                                }
                            }
                        }
                        break;
                    }
                case Settings.ExportResourcesChoice.RAW:
                    {
                        using (FileStream file = File.Create(filename))
                        {
                            // Well, it isn't REALLY raw. We are rewriting it, so of the regular random-header, there are zeros, 
                            //but it is close enough, that information is random anyway
                            this.Write(file);
                            if (progress != null)
                            {
                                progress.SetProgress(progress.GetFullValue());
                            }
                        }
                        break;
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
            for (int i = 0; i < 4; ++i)
            {
                size += (((flag >> (24 + i)) & 1) == 1) ? (baseSize >> (1 + i)) : 0;
            }
            return size;
        }

        static public int GetSizeFromSystemFlag(uint flag)
        {
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
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
            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                return GetSizeFromFlag(flag, 0x1580);
            }
            else
            { // XBOX 360
                return GetSizeFromFlag(flag, 0x2000);
            }
        }

        static public int GetResourceVersionFromFlags(uint systemFlag, uint graphicsFlag)
        {
            return (int)(((graphicsFlag >> 28) & 0xF) | (((systemFlag >> 28) & 0xF) << 4));
        }

        static public bool IsResourceEncrypted(string resourceType)
        {
            // Is xsc is the only encryped resource? Is there a better way to deremine whether it is encrypted or not?
            return resourceType == "sc";
        }
    }
}
