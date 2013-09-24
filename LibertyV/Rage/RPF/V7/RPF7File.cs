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
using LibertyV.Rage.RPF.V7.Entries;
using System.IO.Compression;

namespace LibertyV.Rage.RPF.V7
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

            if (sixteenRoundsDecrypt)
            {
                throw new Exception("Needed to be tested first");
            }

            using (BinaryReader binaryStream = new BinaryReader(AES.DecryptStream(new StreamKeeper(this.Stream), sixteenRoundsDecrypt)))
            {
                MemoryStream entriesInfo = new MemoryStream(binaryStream.ReadBytes(0x10 * Info.EntriesCount));
                MemoryStream filenames = new MemoryStream(binaryStream.ReadBytes(Info.EntriesNamesLength));
                this.Root = Entry.CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), this, entriesInfo, filenames);
            }


            if (!(this.Root is DirectoryEntry))
            {
                throw new Exception("Expected root to be directory");
            }
        }

        public void Write(Stream stream)
        {
            if (this.Filename == "")
            {
                throw new Exception("Can't save");
            }
            // Get all the entries
            List<Entry> entries = new List<Entry>();
            this.Root.AddToList(entries);


            Dictionary<Entry, Structs.RPF7EntryInfoTemplate> entriesInfo = new Dictionary<Entry, Structs.RPF7EntryInfoTemplate>();
            foreach (Entry entry in entries)
            {
                Structs.RPF7EntryInfoTemplate entryInfo = new Structs.RPF7EntryInfoTemplate();
                // update the is resource field
                entryInfo.Field1 = (entry is ResourceEntry) ? 1U : 0U;
                entriesInfo[entry] = entryInfo;
            }

            IEnumerable<string> entriesNames = entries.Select(entry => entry.Name);

            int namesLength = entriesNames.Sum(name => name.Length);
            int shiftNameAccessBy = -1;
            for (int i = 0; i < 8; ++i)
            {
                // the multipcation is the maximum number of nulls, TODO: it can be done better, because it is not always the worst case
                // does the offset should be signed or unsigned? 
                if (namesLength + entriesNames.Count() * (i + 1) < (1 << (16 + i)))
                {
                    shiftNameAccessBy = i;
                    break;
                }
            }

            if (shiftNameAccessBy == -1)
            {
                throw new Exception("Too many entries!");
            }

            int currentPos = 0;
            // Write the names
            stream.Seek(currentPos + 0x10 * (entries.Count + 1), SeekOrigin.Begin);

            using (BinaryWriter writer = new BinaryWriter(AES.EncryptStream(new StreamKeeper(stream), sixteenRoundsDecrypt)))
            {
                foreach (Entry entry in entries)
                {
                    // Update name offset in entry info
                    Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                    entryInfo.Field4 = (uint)currentPos;
                    entriesInfo[entry] = entryInfo;
                    // write name and null, and keep the position
                    writer.Write(entry.Name.ToArray<char>());
                    writer.Write((byte)0);
                    currentPos += entry.Name.Length + 1;
                    // Make the next entry alligned
                    while ((currentPos % (1 << shiftNameAccessBy)) != 0)
                    {
                        writer.Write((byte)0);
                        currentPos += 1;
                    }
                }
                // Align to 0x10 byte
                while (currentPos % 0x10 != 0)
                {
                    writer.Write((byte)0);
                    currentPos += 1;
                }
            }

            // Fill the header
            Info.EntriesCount = entries.Count;
            Info.ShiftNameAccessBy = shiftNameAccessBy;
            Info.EntriesNamesLength = currentPos;

            long[] dataOffsets = new long[entries.Count];
            long[] dataSizes = new long[entries.Count];
            // align to 0x200
            if (stream.Position % (1 << 9) != 0)
            {
                stream.Write(new byte[(1 << 9) - ((int)stream.Position % (1 << 9))], 0, (1 << 9) - ((int)stream.Position % (1 << 9)));
            }
            // Write data and fill entry info
            // TODO: I don't like that the reading logic happens in Entry and the writing logic here
            // I think that I should move the reading logic to here, and split things to functions
            foreach (Entry entry in entries)
            {
                if (entry is FileEntry)
                {
                    long entryOffset = stream.Position;
                    (entry as FileEntry).Write(stream);
                    int entrySize = (int)(stream.Position - entryOffset);
                    // align to 0x200
                    if (stream.Position % (1 << 9) != 0)
                    {
                        stream.Write(new byte[(1 << 9) - ((int)stream.Position % (1 << 9))], 0, (1 << 9) - ((int)stream.Position % (1 << 9)));
                    }

                    // Update the entry offset and entry size
                    Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                    entryInfo.Field2 = (uint)((entryOffset >> 9) & 0x7FFFFF);

                    if (entry is ResourceEntry)
                    {
                        entryInfo.Field3 = (uint)entrySize & 0xFFFFFF;
                        entryInfo.Field5 = (entry as ResourceEntry).SystemFlag;
                        entryInfo.Field6 = (entry as ResourceEntry).GraphicsFlag;
                    }
                    else if (entry is RegularFileEntry)
                    {
                        if ((entry as RegularFileEntry).Compressed)
                        {
                            // The compressed size
                            entryInfo.Field3 = (uint)entrySize & 0xFFFFFF;
                            // The uncompress size
                            entryInfo.Field5 = (uint)(entry as RegularFileEntry).Data.GetSize();
                            // The is encrypted flag
                            entryInfo.Field6 = 1;
                        }
                        else
                        {
                            entryInfo.Field3 = 0;
                            entryInfo.Field5 = (uint)entrySize;
                            // The is encrypted flag
                            entryInfo.Field6 = 0;
                        }
                    }

                    entriesInfo[entry] = entryInfo;

                }
                else
                {
                    Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                    // The offset is "-1" in case of file
                    entryInfo.Field2 = 0x7FFFFF;
                    entriesInfo[entry] = entryInfo;
                }
            }

            using (Stream writer = AES.EncryptStream(new StreamKeeper(stream), sixteenRoundsDecrypt))
            {
                int currentFreeIndex = 1;
                // Last finish to build and write the file
                stream.Seek(0, SeekOrigin.Begin);
                Info.Write(stream);
                Queue<Entry> entriesToProcess = new Queue<Entry>();
                entriesToProcess.Enqueue(this.Root);
                while (entriesToProcess.Count > 0)
                {
                    Entry entry = entriesToProcess.Dequeue();
                    if (entry is DirectoryEntry)
                    {
                        Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                        IList<Entry> subentries = (entry as DirectoryEntry).GetEntries();
                        // Update the info on the sub entries
                        entryInfo.Field5 = (uint)currentFreeIndex;
                        entryInfo.Field6 = (uint)subentries.Count;
                        entriesInfo[entry] = entryInfo;
                        // Process the sub entries
                        foreach (Entry subentry in subentries)
                        {
                            entriesToProcess.Enqueue(subentry);
                        }
                        currentFreeIndex += subentries.Count;
                    }
                    // Write the entry info
                    entriesInfo[entry].Write(writer);
                }
            }
            // Done!
        }
    }
}
