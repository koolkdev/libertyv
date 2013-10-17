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
using System.Windows.Forms;

namespace LibertyV.Operations
{
    static class Operations
    {
        public class SeperatorClass {};
        static SeperatorClass Seperator = new SeperatorClass();

        public static MenuItemsList<LibertyV> MenuStrip = new MenuItemsList<LibertyV>()
        {
            {"File", new MenuItemsList<LibertyV>() {
                {"Open", MainFileOperations.Open, Keys.Control | Keys.O, false, MainFileOperations.CanOpen, false},
                {"Save", MainFileOperations.Save, Keys.Control | Keys.S, false, MainFileOperations.CanSave, false},
                {"Save As...", MainFileOperations.SaveAs, Keys.None, false, MainFileOperations.CanSave, false},
                {"Close", MainFileOperations.Close, Keys.None, false, MainFileOperations.CanSave, false},
                Seperator,
                {"Exit", MainFileOperations.Exit, Keys.Alt | Keys.F4}
            }}
        };

        public static MenuItemsList<FileEntry> FileOperations = new MenuItemsList<FileEntry>(){
            {"Open RPF", RPFOperations.OpenRPF, Keys.None, true, RPFOperations.IsRPF},
            {"Export to wav...", AWCOperations.ExportAWC, Keys.None, false, AWCOperations.IsAWC},
            {"Export file...", Export.ExportFile},
            Seperator,
            {"Rename", Rename.RenameFile, Keys.F2},
            {"Delete", Delete.AskDeleteFile, Keys.Delete},
            {"Properties", FileProperties.ShowFileProperties}
        };

        public static MenuItemsList<FileEntry> ShortcutsFileOperations = new MenuItemsList<FileEntry>(){
            {"Force Delete", Delete.ForceDeleteFile, Keys.Shift | Keys.Delete},
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };

        public static MenuItemsList<List<FileEntry>> MultipleFilesOperations = new MenuItemsList<List<FileEntry>>(){
            {"Export to wav...", AWCOperations.ExportAWCs, Keys.None, false, AWCOperations.IsAWCs},
            {"Export files..", Export.ExportFiles},
            Seperator,
            {"Delete", Delete.AskDeleteFiles, Keys.Delete},
        };

        public static MenuItemsList<List<FileEntry>> ShortcutsMultipleFilesOperations = new MenuItemsList<List<FileEntry>>(){
            {"Force Delete", Delete.ForceDeleteFiles, Keys.Shift | Keys.Delete},
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };

        public static MenuItemsList<DirectoryEntry> FolderOperations = new MenuItemsList<DirectoryEntry>(){
            {"Export folder...", Export.ExportFolder},
            Seperator,
            {"New Folder", New.NewFolder },
            {"Delete", Delete.AskDeleteFolder, Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
            {"Rename", Rename.RenameFolder, Keys.F2, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static MenuItemsList<DirectoryEntry> ShortcutsFolderOperations = new MenuItemsList<DirectoryEntry>(){
            {"Force Delete", Delete.ForceDeleteFolder, Keys.Shift | Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static MenuItemsList<DirectoryEntry> FilesListOperations = new MenuItemsList<DirectoryEntry>(){
            {"Import files...", Import.ImportFiles }
        };

        public static MenuItemsList<DirectoryEntry> ShortcutsFilesListOperations = new MenuItemsList<DirectoryEntry>()
        {
            {"Select All", Helper.SelectAll, Keys.Control | Keys.A}
        };
        public static void PerformDefaultAction<T>(MenuItemsList<T> operations, T obj)
        {
            Action<T> action = operations.GetDefaultAction(obj);
            if (action != null)
            {
                action(obj);
            }
        }

        public static bool PerformActionByKey<T>(MenuItemsList<T> operations, MenuItemsList<T> shortcutOperations, Keys key, T obj)
        {
            Action<T> action = operations.GetActionByKey(key, obj);
            if (action != null)
            {
                action(obj);
                return true;
            }
            action = shortcutOperations.GetActionByKey(key, obj);
            if (action != null)
            {
                action(obj);
                return true;
            }
            return false;
        }
    }
}
