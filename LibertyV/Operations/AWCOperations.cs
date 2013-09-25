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
            if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                MessageBox.Show("Not supported yet on Xbox");
                return;
            }
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                try
                {
                    using (AWCFile awc = new AWCFile(entry.Data.GetStream()))
                    {
                        awc.ExportWav(Path.Combine(selectedFolder, entry.Name));
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to open AWC, please report to the developer");
                }
            }
        }

        public static void ExportAWCs(List<FileEntry> entries)
        {
            if (GlobalOptions.Platform == Platform.PlatformType.XBOX360)
            {
                MessageBox.Show("Not supported yet on Xbox");
                return;
            }
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                foreach (FileEntry entry in entries)
                {
                    try
                    {
                        using (AWCFile awc = new AWCFile(entry.Data.GetStream()))
                        {
                            awc.ExportWav(Path.Combine(selectedFolder, entry.Name));
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Failed to open AWC, please report to the developer");
                    }
                }
            }
        }
    }
}
