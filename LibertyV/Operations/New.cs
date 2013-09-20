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
using LibertyV.RPF7.Entries;

namespace LibertyV.Operations
{
    static class New
    {
        public static void NewFolder(DirectoryEntry entry)
        {
            string name = "New Folder";
            int i = 1;
            for (; entry.Entries.Any(e => e.Name == name); ++i, name = String.Format("New Folder ({0})", i));
            DirectoryEntry addedFolder = new DirectoryEntry(name, new List<Entry>());
            entry.Entries.Add(addedFolder);
            addedFolder.Parent = entry;
            entry.Node.Nodes.Add(new EntryTreeNode(addedFolder, new EntryTreeNode[]{}));
            if (!entry.Node.IsExpanded)
            {
                entry.Node.Expand();
            }
            addedFolder.Node.TreeView.SelectedNode = addedFolder.Node;
            addedFolder.Node.TreeView.LabelEdit = true;
            addedFolder.Node.BeginEdit();
        }
    }
}
