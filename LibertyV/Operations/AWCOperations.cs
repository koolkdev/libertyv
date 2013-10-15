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
using LibertyV.Rage.RPF.V7.Entries;
using LibertyV.Rage.Audio.AWC;
using System.IO;
using LibertyV.Utils;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    static class AWCOperations
    {
        public static bool IsAWC(FileEntry entry)
        {
            return entry.GetExtension() == ".awc";
        }

        public static bool IsAWCs(List<FileEntry> entries)
        {
            return !entries.Any(entry => entry.GetExtension() != ".awc");
        }

        public static void ExportAWC(FileEntry entry)
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                try
                {
                    using (AWCFile awc = new AWCFile(entry.Data.GetStream()))
                    {
                        ProgressWindow progress = new ProgressWindow("Exporting", report => awc.ExportWav(Path.Combine(selectedFolder, entry.Name), report), true);
                        progress.Run();
                    }
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to open AWC, please report to the developer");
                }
            }
        }

        public static void ExportAWCs(List<FileEntry> entries)
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                ProgressWindow progress = new ProgressWindow("Exporting", report =>
                {
                    int passed = 0;
                    // Set the progress by the size of the file
                    report = new SubProgressReport(report, entries.Sum(entry => entry.Data.GetSize()));
                    foreach (FileEntry entry in entries)
                    {
                        using (AWCFile awc = new AWCFile(entry.Data.GetStream()))
                        {
                            awc.ExportWav(Path.Combine(selectedFolder, entry.Name), new SubProgressReport(report, passed, entry.Data.GetSize()));
                            passed += entry.Data.GetSize();
                        }
                    }
                }, true);
                try
                {
                    progress.Run();
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Operation canceled.");
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to open AWC, please report to the developer");
                }
            }
        }
    }
}
