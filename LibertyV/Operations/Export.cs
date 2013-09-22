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

namespace LibertyV.Operations
{
    static class Export
    {

        public static void ExportFile(FileEntry entry)
        {
            string selectedFilename;
            // Open a folder selection instead of file selection only if there is need to.
            if (entry is ResourceEntry && Properties.Settings.Default.ExportResourcesChoice == Settings.ExportResourcesChoice.SYSGFX && ((ResourceEntry)entry).GraphicSize > 0)
            {
                selectedFilename = GUI.FolderSelection();
            }
            else
            {
                selectedFilename = GUI.FileSaveSelection(entry.Name);
            }
            if (selectedFilename != null)
            {
                entry.Export(selectedFilename);
            }
        }

        public static void ExportFiles(ICollection<FileEntry> entries)
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                foreach (FileEntry entry in entries)
                {
                    entry.Export(selectedFolder);
                }
            }
        }

        public static void ExportFolder(DirectoryEntry entry)
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                entry.Export(selectedFolder);
            }
        }
    }
}
