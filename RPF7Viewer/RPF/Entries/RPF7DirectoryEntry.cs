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
using RPF7Viewer.Utils;

namespace RPF7Viewer.RPF.Entries
{
    public class RPF7DirectoryEntry : RPF7Entry
    {
        public List<RPF7Entry> Entries;

        public RPF7DirectoryEntry(String filename, List<RPF7Entry> entries) : base(filename)
        {
            this.Entries = entries;
            foreach (RPF7Entry entry in this.Entries) {
                entry.Parent = this;
            }
        }

        public RPF7TreeNode GetTreeNodes() {
            List<RPF7TreeNode> children = new List<RPF7TreeNode>();
            foreach (RPF7Entry entry in this.Entries)
            {
                if (entry is RPF7DirectoryEntry)
                {
                    children.Add((entry as RPF7DirectoryEntry).GetTreeNodes());
                }
            }
            return new RPF7TreeNode(this, children.ToArray());
        }

        public List<RPF7ListViewItem> GetListViewItems()
        {
            List<RPF7ListViewItem> children = new List<RPF7ListViewItem>();
            foreach (RPF7Entry entry in this.Entries)
            {
                if (entry is RPF7FileEntry)
                {
                    children.Add(new RPF7ListViewItem(entry as RPF7FileEntry));
                }
            }
            return children;
        }

        public override void Export(String foldername)
        {
            String subfolder = Path.Combine(foldername, this.Filename);
            Directory.CreateDirectory(subfolder);
            foreach (RPF7Entry entry in this.Entries)
            {
                entry.Export(subfolder);
            }
        }
    }
}
