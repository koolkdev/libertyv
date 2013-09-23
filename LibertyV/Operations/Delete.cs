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
using LibertyV.RPF.V7.Entries;
using LibertyV.Utils;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    static class Delete
    {
        public static void ForceDeleteFolder(DirectoryEntry entry)
        {
            DeleteFolder(entry);
        }
        public static void ForceDeleteFile(FileEntry entry)
        {
            DeleteFile(entry);
        }
        public static void ForceDeleteFiles(ICollection<FileEntry> entries)
        {
            DeleteFiles(entries);
        }

        public static void AskDeleteFolder(DirectoryEntry entry)
        {
            DeleteFolder(entry, false);
        }
        public static void AskDeleteFile(FileEntry entry)
        {
            DeleteFile(entry, false);
        }
        public static void AskDeleteFiles(ICollection<FileEntry> entries)
        {
            DeleteFiles(entries, false);
        }

        public static void DeleteFolder(DirectoryEntry entry, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete the folder '{0}'?", entry.Name), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                entry.Parent.RemoveEntry(entry);
            }
        }

        public static void DeleteFile(FileEntry entry, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete the item '{0}'?", entry.Name), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                entry.Parent.RemoveEntry(entry);
            }
        }

        public static void DeleteFiles(ICollection<FileEntry> entries, bool force = true)
        {
            if (force || MessageBox.Show(String.Format("Are you sure you want to delete {0} items?", entries.Count), "Delete Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                foreach (FileEntry entry in entries)
                {
                    entry.Parent.RemoveEntry(entry);
                }
            }
        }
    }
}
