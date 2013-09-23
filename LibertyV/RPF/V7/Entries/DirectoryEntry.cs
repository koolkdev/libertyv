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
    public class DirectoryEntry : Entry
    {
        private SortedList<string, Entry> Entries;
        public EntryTreeNode Node = null;
        public System.Windows.Forms.ListView FilesListView = null;

        public DirectoryEntry(String filename, List<Entry> entries)
            : base(filename)
        {
            this.Entries = new SortedList<string, Entry>();
            foreach (Entry entry in entries)
            {
                this.Entries.Add(entry.Name, entry);
                entry.Parent = this;
            }
        }

        public IList<Entry> GetEntries()
        {
            return Entries.Values;
        }

        public Entry GetEntry(string name)
        {
            // Clean the input (for the case it is /dir/)
            while (name.Length > 0 && (name[0] == '\\' || name[0] == '/'))
            {
                name = name.Substring(1);
            }
            if (name == "")
            {
                return this;
            }

            int dirSplit = name.IndexOfAny(new char[] { '\\', '/' });
            Entry entry = null;
            if (dirSplit == -1)
            {
                // No directory
                this.Entries.TryGetValue(name, out entry);
                return entry;
            }
            this.Entries.TryGetValue(name.Substring(0, dirSplit), out entry);
            DirectoryEntry dirEntry = entry as DirectoryEntry;
            if (dirEntry != null)
            {
                return dirEntry.GetEntry(name.Substring(dirSplit + 1));
            }
            return null;
        }

        public void AddEntry(Entry entry)
        {
            Entries.Add(entry.Name, entry);
            entry.Parent = this;
            // Add to GUI if needed
            if (entry is FileEntry)
            {
                if (this.FilesListView != null)
                {
                    this.FilesListView.Items.Add(new EntryListViewItem(entry as FileEntry));
                }
            }
            else
            {
                this.Node.Nodes.Add(new EntryTreeNode(entry as DirectoryEntry, new EntryTreeNode[] { }));
            }
        }

        public void RemoveEntry(Entry entry)
        {
            Entries.Remove(entry.Name);
            // Remove from GUI if needed
            if (entry is FileEntry)
            {
                if (((FileEntry)entry).ViewItem != null)
                {
                    ((FileEntry)entry).ViewItem.Remove();
                }
            }
            else
            {
                ((DirectoryEntry)entry).Node.Remove();
            }
        }
        public bool IsRoot()
        {
            return Parent == null;
        }

        public override void Export(String foldername)
        {
            String subfolder = Path.Combine(foldername, this.Name);
            Directory.CreateDirectory(subfolder);
            foreach (Entry entry in this.Entries.Values)
            {
                entry.Export(subfolder);
            }
        }

        public override void AddToList(List<Entry> entryList)
        {
            base.AddToList(entryList);
            foreach (Entry entry in this.Entries.Values)
            {
                entry.AddToList(entryList);
            }
        }
    }
}
