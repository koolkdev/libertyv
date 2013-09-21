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

namespace LibertyV.RPF7.Entries
{
    public class DirectoryEntry : Entry
    {
        public List<Entry> Entries;
        public EntryTreeNode Node = null;
        public System.Windows.Forms.ListView FilesListView = null; 

        public DirectoryEntry(String filename, List<Entry> entries) : base(filename)
        {
            this.Entries = entries;
            foreach (Entry entry in this.Entries) {
                entry.Parent = this;
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
            foreach (Entry entry in this.Entries)
            {
                entry.Export(subfolder);
            }
        }
    }
}
