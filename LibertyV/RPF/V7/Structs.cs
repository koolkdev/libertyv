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
using System.Diagnostics.Contracts;
using LibertyV.Utils;

namespace LibertyV.RPF.V7
{
    public static class Structs
    {
        public static uint SwapEndian(uint num)
        {
            return (num >> 24) | ((num >> 8) & 0xff00) | ((num & 0xff00) << 8) | ((num & 0xff) << 24);
        }

        public struct RPF7Header
        {
            public char[] Magic;
            public int EntriesCount;
            public int PlatformBit;
            public int ShiftNameAccessBy;
            public int EntriesNamesLength;
            public uint Flag;

            public RPF7Header(int entriesCount, int shiftNameAccessBy, int entriesNamesLength, uint flag)
            {
                Magic = new char[] { 'R', 'P', 'F', '7' };
                EntriesCount = entriesCount;
                if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
                {
                    PlatformBit = 0;
                }
                else
                {
                    PlatformBit = 1;
                }
                ShiftNameAccessBy = shiftNameAccessBy;
                EntriesNamesLength = entriesNamesLength;
                Flag = flag;
            }

            public RPF7Header(Stream stream)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream))) {
                    Magic = new char[4];
                    s.Read(Magic, 0, 4);
                    EntriesCount = (int)SwapEndian(s.ReadUInt32());
                    uint sizeAndInfo = SwapEndian(s.ReadUInt32());
                    PlatformBit = (int)((sizeAndInfo >> 31) & 1);
                    ShiftNameAccessBy = (int)((sizeAndInfo >> 28) & 7);
                    EntriesNamesLength = (int)(sizeAndInfo & 0x0FFFFFFF);
                    Flag = SwapEndian(s.ReadUInt32());
                }
            }

            public void Write(Stream stream)
            {
                using (BinaryWriter s = new BinaryWriter(new StreamKeeper(stream)))
                {
                    s.Write(Magic);
                    s.Write(SwapEndian((uint)EntriesCount));
                    uint sizeAndInfo = (uint)EntriesNamesLength & 0x0FFFFFFF;
                    sizeAndInfo |= ((uint)ShiftNameAccessBy & 7) << 28;
                    sizeAndInfo |= ((uint)PlatformBit & 1) << 31;
                    s.Write(SwapEndian(sizeAndInfo));
                    s.Write(SwapEndian(Flag));
                }
            }
        }

        public struct RPF7EntryInfoTemplate
        {
            public uint Field1;
            public uint Field2;
            public uint Field3;
            public uint Field4;
            public uint Field5;
            public uint Field6;

            public RPF7EntryInfoTemplate(uint field1, uint field2, uint field3, uint field4, uint field5, uint field6)
            {
                Field1 = field1;
                Field2 = field2;
                Field3 = field3;
                Field4 = field4;
                Field5 = field5;
                Field6 = field6;
            }

            public RPF7EntryInfoTemplate(Stream stream)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream))) {
                    uint field1and2 = (uint)s.ReadByte() << 16;
                    field1and2 |= (uint)s.ReadByte() << 8;
                    field1and2 |= (uint)s.ReadByte();
                    Field1 = (field1and2 >> 23) & 1;
                    Field2 = field1and2 & 0x7FFFFF;
                    Field3 = (uint)s.ReadByte() << 16;
                    Field3 |= (uint)s.ReadByte() << 8;
                    Field3 |= (uint)s.ReadByte();
                    Field4 = (uint)s.ReadByte() << 8;
                    Field4 |= (uint)s.ReadByte();
                    Field5 = SwapEndian(s.ReadUInt32());
                    Field6 = SwapEndian(s.ReadUInt32());
                }
            }

            public void Write(Stream stream)
            {
                using (BinaryWriter s = new BinaryWriter(new StreamKeeper(stream)))
                {
                    uint field1and2 = (Field2 & 0x7FFFFF) | ((Field1 & 1) << 23);
                    s.Write((byte)((field1and2 >> 16) & 0xFF));
                    s.Write((byte)((field1and2 >> 8) & 0xFF));
                    s.Write((byte)(field1and2 & 0xFF));
                    s.Write((byte)((Field3 >> 16) & 0xFF));
                    s.Write((byte)((Field3 >> 8) & 0xFF));
                    s.Write((byte)(Field3 & 0xFF));
                    s.Write((byte)((Field4 >> 8) & 0xFF));
                    s.Write((byte)(Field4 & 0xFF));
                    s.Write(SwapEndian(Field5));
                    s.Write(SwapEndian(Field6));
                }
            }
        }
    }
}
