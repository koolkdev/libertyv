using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LibertyV.Utils;

namespace LibertyV.Rage.Audio.AWC
{
    class Structs
    {

        public static ushort SwapEndian(ushort num)
        {
            return (ushort)((num >> 8) | ((num & 0xff) << 8));
        }

        public static uint SwapEndian(uint num)
        {
            return (num >> 24) | ((num >> 8) & 0xff00) | ((num & 0xff00) << 8) | ((num & 0xff) << 24);
        }

        public static ulong SwapEndian(ulong num)
        {
            return (num >> 56) | ((num >> 40) & 0xff00) | ((num >> 24) & 0xff0000) | ((num >> 8) & 0xff000000) | ((num & 0xff000000) << 8) | ((num & 0xff0000) << 24) | ((num & 0xff00) << 40) | ((num & 0xff) << 56);
        }

        private static ushort FixEndian(ushort num, bool bigEndian)
        {
            if (bigEndian)
            {
                return SwapEndian(num);
            }
            return num;
        }

        private static uint FixEndian(uint num, bool bigEndian)
        {
            if (bigEndian)
            {
                return SwapEndian(num);
            }
            return num;
        }

        private static ulong FixEndian(ulong num, bool bigEndian)
        {
            if (bigEndian)
            {
                return SwapEndian(num);
            }
            return num;
        }

        public class AWCHeader
        {
            public bool BigEndian;

            public char[] Magic;

            public short Version;
            public ushort Flags;

            public int StreamsCount;
            public int StreamsInfoOffset;

            public AWCHeader(Stream stream)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    Magic = new char[4];
                    s.Read(Magic, 0, 4);

                    if (new string(Magic) == "TADA")
                    {
                        BigEndian = true;
                    }
                    else if (new string(Magic) == "ADAT")
                    {
                        BigEndian = false;
                    }
                    else
                    {
                        throw new Exception("Invalid awc magic");
                    }

                    uint versionAndFlags = FixEndian(s.ReadUInt32(), BigEndian);
                    Version = (short)(versionAndFlags & 0xFFFF);
                    Flags = (ushort)((versionAndFlags >> 16) & 0xFFFF);
                    StreamsCount = (int)FixEndian(s.ReadUInt32(), BigEndian);
                    StreamsInfoOffset = (int)FixEndian(s.ReadUInt32(), BigEndian);
                }
            }
        }

        public class StreamInfo
        {
            public int TagsCount;
            public int Id;

            public StreamInfo(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    uint info = FixEndian(s.ReadUInt32(), bigEndian);
                    Id = (int)(info & 0x1fffffff);
                    TagsCount = (int)(info >> 29);
                }
            }
        }

        public class ChunkInfo
        {
            public byte Tag;
            public int Offset;
            public int Size;

            public ChunkInfo(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    ulong info = FixEndian(s.ReadUInt64(), bigEndian);
                    Tag = (byte)(info >> 56);
                    Offset = (int)(info & 0x0fffffff);
                    Size = (int)((info >> 28) & 0x0fffffff);
                }
            }
        }

        public class FormatChunk
        {
            public uint Samples;
            public int unknownMinusOne;
            public ushort SamplesPerSecond;
            public ushort unknownWord1;
            public ushort unknownWord2;
            public ushort unknownWord3;
            public ushort unknownWord4;
            public byte unknownByte1;
            public byte unknownByte2;

            public FormatChunk(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    Samples = FixEndian(s.ReadUInt32(), bigEndian);
                    unknownMinusOne = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    SamplesPerSecond = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownWord1 = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownWord2 = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownWord3 = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownWord4 = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownByte1 = s.ReadByte();
                    unknownByte2 = s.ReadByte();

                    // Sometimes those words shop up and somteims not
                    //FixEndian(s.ReadUInt16(), bigEndian);
                    //FixEndian(s.ReadUInt16(), bigEndian);
                }
            }

            public FormatChunk(ChannelsInfoChunkItem channelInfo)
            {
                Samples = channelInfo.Samples;
                unknownMinusOne = -1;
                SamplesPerSecond = channelInfo.SamplesPerSecond;
                unknownWord1 = channelInfo.unknownWord1;
                unknownWord2 = channelInfo.unknownWord2;
                unknownWord3 = 0;
                unknownWord4 = 0;
                unknownByte1 = 0;
                unknownByte2 = channelInfo.unknownByte2;
            }
        }

        public class ChannelsInfoChunkHeader
        {
            public int Unknown1; // small number
            public int BigChunkSize;
            public int ChannelsCount;

            public ChannelsInfoChunkHeader(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    Unknown1 = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    BigChunkSize = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    ChannelsCount = (int)FixEndian(s.ReadUInt32(), bigEndian);
                }
            }
        }

        public class ChannelsInfoChunkItem
        {
            public int Id;
            public uint Samples; // big number
            public ushort unknownWord1;
            public ushort SamplesPerSecond;
            public byte unknownByte2;
            public byte RoundSize; 
            public ushort unknownWord2;

            public ChannelsInfoChunkItem(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    Id = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    Samples = FixEndian(s.ReadUInt32(), bigEndian);
                    unknownWord1 = FixEndian(s.ReadUInt16(), bigEndian);
                    SamplesPerSecond = FixEndian(s.ReadUInt16(), bigEndian);
                    unknownByte2 = s.ReadByte();
                    RoundSize = s.ReadByte();
                    unknownWord2 = FixEndian(s.ReadUInt16(), bigEndian);
                }
            }
        }

        public class ChannelChunkHeader
        {
            public static int Size
            {
                get
                {
                    if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
                    {
                        // For some reason the playstation 3 have bigger header, probably codec specific?
                        return 0x18;
                    }
                    else
                    {
                        return 0x10;
                    }
                }
            }
            // zero most of the time
            public int SamplesSkip;
            public int StartChunk;
            public int Chunks;
            // Samples in chunk
            public uint Samples;
            public int DataSize;

            public ChannelChunkHeader(Stream stream, bool bigEndian)
            {
                using (BinaryReader s = new BinaryReader(new StreamKeeper(stream)))
                {
                    StartChunk = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    Chunks = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    SamplesSkip = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    Samples = FixEndian(s.ReadUInt32(), bigEndian);

                    if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
                    {
                        // PS3 has another 8 unknown bytes (2 dwords)
                        s.ReadInt32();
                        DataSize = (int)FixEndian(s.ReadUInt32(), bigEndian);
                    }
                    else
                    {
                        DataSize = Chunks * 0x800;
                    }
                }
            }
        }
    }
}
