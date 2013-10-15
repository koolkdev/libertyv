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
    public class RPF7File : IDisposable
    {
        public Stream Stream;

        public Structs.RPF7Header Info;
        private bool sixteenRoundsDecrypt;

        public DirectoryEntry Root;
        public String Filename;

        public RPF7File(Stream inputStream, String filname = "")
        {
            if (!inputStream.CanRead)
            {
                throw new RPFParsingException("Stream isn't readable");
            }
            this.Filename = filname;
            Stream = inputStream;

            try
            {
                Info = new Structs.RPF7Header(Stream);
            }
            catch (Exception e)
            {
                throw new RPFParsingException(String.Format("Failed to read header: {0}", e.Message));
            }

            if (new string(Info.Magic) != "RPF7")
            {
                throw new RPFParsingException("Invalid RPF Magic");
            }

            if (GlobalOptions.Platform == Platform.PlatformType.PLAYSTATION3)
            {
                if (Info.PlatformBit != 0)
                {
                    throw new RPFParsingException("Invalid platform bit (Are you sure that this RPF is for PlayStation 3?)");
                }
            }
            else if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                if (Info.PlatformBit != 1)
                {
                    throw new RPFParsingException("Invalid platform bit (Are you sure that this RPF is for Xbox 360?)");
                }
            }

            if (Info.EntriesCount == 0)
            {
                throw new RPFParsingException("Empty RPF - no root directory");
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
                Range<Boolean> fileUsage = new Range<Boolean>(Stream.Length);
                // Just mark the root entry info and the header as used
                fileUsage.AddItem(0, 0x10 + 0x10, true);
                this.Root = CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), entriesInfo, filenames, fileUsage) as DirectoryEntry;
            }


            if (this.Root == null)
            {
                throw new RPFParsingException("Expected root to be a directory.");
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Root.Dispose();
            this.Stream.Close();
        }

        private Entry CreateFromHeader(Structs.RPF7EntryInfoTemplate entryInfo, MemoryStream entriesInfo, MemoryStream filenames, Range<Boolean> fileUsage)
        {
            // Todo: check sizes and offsets
            bool isResource = entryInfo.Field1 == 1;
            long offset = (long)entryInfo.Field2;
            int compressedSize = (int)entryInfo.Field3;
            int filenameOffset = (int)entryInfo.Field4;

            filenameOffset <<= Info.ShiftNameAccessBy;

            if (filenameOffset < 0 || filenameOffset >= filenames.Length)
            {
                throw new RPFEntryParsingException("Reading entry name: Invalid offst");
            }
            filenames.Seek(filenameOffset, SeekOrigin.Begin);

            String filename = "";
            // Read null-terminated filename
            int currentChar;
            while ((currentChar = filenames.ReadByte()) != 0)
            {
                if (currentChar == -1)
                {
                    throw new RPFEntryParsingException("Reading entry name: Unexpected EOF");
                }
                filename += (char)currentChar;
            }
            // There may be duplicate names usage, so don't check it
            if (!fileUsage.AddItem(0x10 + entriesInfo.Length + filenameOffset, filename.Length + 1, true))
            {
                throw new RPFEntryParsingException("Reading entry entry: Invalid duplicate usage of a name");
            }

            if (offset == 0x7FFFFF)
            {
                // Is a Directory
                if (isResource)
                {
                    throw new RPFEntryParsingException("Invalid entry type (directory and resource)");
                }
                int subentriesStartIndex = (int)entryInfo.Field5;
                int subentriesCount = (int)entryInfo.Field6;
                if (subentriesStartIndex < 0 || (subentriesStartIndex + subentriesCount) * 0x10 > entriesInfo.Length)
                {
                    throw new RPFEntryParsingException("Invalid directory entry: Invalid subentries info");
                }

                List<Entry> entries = new List<Entry>();
                // This will prevent recursion..
                if (!fileUsage.AddItem(0x10 * (1 + subentriesStartIndex), 0x10 * subentriesCount, true, false))
                {
                    throw new RPFEntryParsingException("Reading directory entry: Duplicate usage of an entries - may lead to recursion");
                }

                for (int i = 0; i < subentriesCount; ++i)
                {
                    entriesInfo.Seek(0x10 * (i + subentriesStartIndex), SeekOrigin.Begin);
                    entries.Add(CreateFromHeader(new Structs.RPF7EntryInfoTemplate(entriesInfo), entriesInfo, filenames, fileUsage));
                }
                return new DirectoryEntry(filename, entries);
            }

            offset <<= 9;

            if (offset < 0 || offset > Stream.Length)
            {
                throw new RPFEntryParsingException("Invalid entry info: Invalid data offset");
            }

            if (isResource)
            {
                if (compressedSize == 0xFFFFFF)
                {
                    throw new RPFEntryParsingException("Resource with size -1, not supported (Contact developr if seen)");
                }
                if (!fileUsage.AddItem(offset, compressedSize, true, false)) // Can't use the same data for two files
                {
                    throw new RPFEntryParsingException("Reading entry data: Data position is wierd");
                }
                uint systemFlag = entryInfo.Field5;
                uint graphicsFlag = entryInfo.Field6;
                return new ResourceEntry(filename, new ResourceStreamCreator(Stream, offset, compressedSize, systemFlag, graphicsFlag, Path.GetExtension(filename).Substring(2)), systemFlag, graphicsFlag);
            }

            // Regular file
            int uncompressedSize = (int)entryInfo.Field5;
            int isEncrypted = (int)entryInfo.Field6;

            if (compressedSize == 0)
            {
                // Uncompressed file
                if (isEncrypted != 0)
                {
                    throw new RPFEntryParsingException("Unexcepted file - compressed but unencrypted (Contact developr if seen)");
                }
                if (!fileUsage.AddItem(offset, uncompressedSize, true, false)) // Can't use the same data for two files
                {
                    throw new RPFEntryParsingException("Reading entry data: Data position is wierd");
                }
                return new RegularFileEntry(filename, new FileStreamCreator(Stream, offset, uncompressedSize), false);
            }
            else
            {
                if (!fileUsage.AddItem(offset, compressedSize, true, false)) // Can't use the same data for two files
                {
                    throw new RPFEntryParsingException("Reading entry data: Data position is wierd");
                }
                // Compressed file
                return new RegularFileEntry(filename, new CompressedFileStreamCreator(Stream, offset, compressedSize, uncompressedSize, isEncrypted != 0), true);
            }
        }

        private MemoryStream WriteNames(List<Entry> entries, Dictionary<Entry, Structs.RPF7EntryInfoTemplate> entriesInfo, out int shiftNameAccessBy)
        {
            IEnumerable<string> entriesNames = entries.Select(entry => entry.Name);

            int namesLength = entriesNames.Sum(name => name.Length);
            shiftNameAccessBy = -1;
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

            MemoryStream filenames = new MemoryStream();
            SortedList<string, int> WritedNames = new SortedList<string, int>();

            using (BinaryWriter writer = new BinaryWriter(new StreamKeeper(filenames)))
            {
                foreach (Entry entry in entries)
                {
                    int nameOffset;

                    // Check if we already wrote this name, if it is use it
                    if (!WritedNames.TryGetValue(entry.Name, out nameOffset))
                    {
                        nameOffset = currentPos;
                        WritedNames[entry.Name] = nameOffset;

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

                    // Update name offset in entry info
                    Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                    entryInfo.Field4 = (uint)nameOffset;
                    entriesInfo[entry] = entryInfo;
                }
                // Align to 0x10 byte
                while (currentPos % 0x10 != 0)
                {
                    writer.Write((byte)0);
                    currentPos += 1;
                }
            }

            return filenames;
        }

        private void WriteData(Stream stream, List<Entry> entries, Dictionary<Entry, Structs.RPF7EntryInfoTemplate> entriesInfo, IProgressReport progressReport = null)
        {
            if (progressReport != null)
            {
                progressReport = new SubProgressReport(progressReport, entries.Sum(entry => entry is FileEntry ? ((FileEntry)entry).Data.GetSize() : 0));
            }

            int passed = 0;
            // Write data and fill entry info
            foreach (Entry entry in entries)
            {
                if (progressReport != null && progressReport.IsCanceled())
                {
                    throw new OperationCanceledException();
                }
                if (entry is FileEntry)
                {
                    if (progressReport != null)
                    {
                        progressReport.SetMessage(String.Format("Writing file {0}.", entry.Name));
                    }
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

                    if (progressReport != null)
                    {
                        passed += (entry as FileEntry).Data.GetSize();
                        progressReport.SetProgress(passed);
                    }

                }
                else
                {
                    Structs.RPF7EntryInfoTemplate entryInfo = entriesInfo[entry];
                    // The offset is "-1" in case of file
                    entryInfo.Field2 = 0x7FFFFF;
                    entriesInfo[entry] = entryInfo;
                }
            }
        }

        /* TODO:
        private void WriteDataInline(Stream stream, List<Entry> entries, Dictionary<Entry, Structs.RPF7EntryInfoTemplate> entriesInfo, int headersEnd)
        {
            Range<FileEntry> oldFileLayout = new Range<FileEntry>(Math.Max(stream.Length, (long)headersEnd));
            List<FileEntry> emptyEntries = new List<FileEntry>();
            List<FileEntry> moveEntries = new List<FileEntry>();
            List<FileEntry> newEntries = new List<FileEntry>();
            // Entries that should be written to temporary files 
            List<FileEntry> newSavedEntries = new List<FileEntry>();

            // A dictionary of the raw data stream that we need to write to the file
            Dictionary<FileEntry, Stream> entriesStream = new Dictionary<FileEntry, System.IO.Stream>();

            // Add the header
            oldFileLayout.AddItem(0, headersEnd, null);

            foreach (Entry entry in entries)
            {
                FileEntry fentry = entry as FileEntry;
                if (fentry != null)
                {

                    // We need to find all the files that depends on the origianl stream first
                    FileStreamCreator originalStream = fentry.TryGetOriginalFileStreamCreator();
                    if (fentry.Data.GetSize() == 0 && fentry.IsRegularFile() && !((RegularFileEntry)fentry).Compressed)
                    {
                        emptyEntries.Add(fentry);
                    }
                    else if (originalStream != null && originalStream.FileStream == this.Stream)
                    {
                        if (originalStream.Offset < headersEnd) {
                            moveEntries.Add(fentry);
                        }
                        if (!oldFileLayout.AddItem(originalStream.Offset, originalStream.Size, fentry, false))
                        {
                            // If failed for some reason.. well it can happen if two entries have the same data. Anyway I don't want that two entries will use the same data.
                            newSavedEntries.Add(fentry);
                        }
                        else
                        {
                            entriesStream[fentry] = new PartialStream(this.Stream, originalStream.Offset, originalStream.Size);
                        }
                    }
                    else if (fentry.Data is FileStreamCreator && ((FileStreamCreator)fentry.Data).FileStream == this.Stream)
                    {
                        // This file points to the original file, but something about it changed (compression, encryption, ..)
                        newSavedEntries.Add(fentry);
                    }
                    // From here are entries that are not depends on the original strema
                    else if ((fentry.IsRegularFile() && ((RegularFileEntry)fentry).Compressed) || fentry.IsResource())
                    {
                        newSavedEntries.Add(fentry);
                    }
                    else
                    {
                        entriesStream[fentry] = fentry.Data.GetStream();
                        newEntries.Add(fentry);
                    }
                }
            }

            foreach (FileEntry entry in newSavedEntries) 
            {
                string filepath = Path.GetTempFileName();
                // Let's copy the file to temp folder, this file will be deleted on close
                FileStream writeStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 0x1000, FileOptions.DeleteOnClose);

                entry.Data.GetStream().CopyTo(writeStream);

                // Take an handle to the file, so it will be delete only when this handle will be closed.
                FileStream readStream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                writeStream.Close();

                entriesStream[entry] = writeStream;
            }

            Range<FileEntry> newFileLayout = new Range<FileEntry>();
        }
         */

        public void Write(Stream stream, IProgressReport progressReport = null)
        {
            if (progressReport != null)
            {
                progressReport.SetMessage("Preparing...");
                progressReport.SetProgress(-1);
            }
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

            int shiftNameAccessBy;
            Stream names = WriteNames(entries, entriesInfo, out shiftNameAccessBy);

            // Fill the header
            Info.EntriesCount = entries.Count;
            Info.ShiftNameAccessBy = shiftNameAccessBy;
            Info.EntriesNamesLength = (int)names.Length;

            // Go to current position
            stream.Seek(0x10 + 0x10 * Info.EntriesCount + Info.EntriesNamesLength, SeekOrigin.Begin);

            // align to 0x200
            if (stream.Position % (1 << 9) != 0)
            {
                stream.Write(new byte[(1 << 9) - ((int)stream.Position % (1 << 9))], 0, (1 << 9) - ((int)stream.Position % (1 << 9)));
            }
            WriteData(stream, entries, entriesInfo, progressReport);

            long end = stream.Position;

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

                // Write names
                names.Seek(0, SeekOrigin.Begin);
                names.CopyTo(writer);
            }

            // Seek to end
            stream.Seek(end, SeekOrigin.Begin);
            // Done!
        }
    }
}
