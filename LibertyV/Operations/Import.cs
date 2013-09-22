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
using System.Windows.Forms;
using LibertyV.Utils;
using System.IO;
using LibertyV.RPF.V7;
using LibertyV.RPF;

namespace LibertyV.Operations
{
    static class Import
    {
        public static void ImportFiles(DirectoryEntry entry)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK) {
                foreach (String file in fileDialog.FileNames)
                {
                    if (entry.GetEntries().Any(e => e.Name == Path.GetFileName(file)))
                    {
                        // TODO: Ask for overwrite
                        MessageBox.Show(String.Format("Error: file {0} already exists.", Path.GetFileName(file)));
                        return;
                    }
                }
                foreach (String file in fileDialog.FileNames)
                {
                    // TODO: add resources, decide if to compress or not, all by extentions.
                    // Right now all regular files compressed by default
                    RegularFileEntry addedFile = new RegularFileEntry(Path.GetFileName(file), new ExternalFileStreamCreator(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read)), true);
                    entry.AddEntry(addedFile);
                    if (entry.FilesListView != null)
                    {
                        entry.FilesListView.Items.Add(new EntryListViewItem(addedFile));
                    }
                }
            }
        }
        public static void ImportFolder(DirectoryEntry entry)
        {
            string folder = GUI.FolderSelection();
        }
    }
}
