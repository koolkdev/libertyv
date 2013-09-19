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
using System.Windows.Forms;

namespace LibertyV.Operations
{
    static class Operations
    {

        public static OperationsList<FileEntry> FileOperations = new OperationsList<FileEntry>(){
            {"Open RPF", RPFOperations.OpenRPF, Keys.None, true, RPFOperations.IsRPF},
            {"Export file...", Export.ExportFile},
            {"Rename", Rename.RenameFile, Keys.F2},
            {"Delete", Delete.AskDeleteFile, Keys.Delete},
            {"Proporties", FileProporties.ShowFileProporties}
        };

        public static OperationsList<FileEntry> ShortcutsFileOperations = new OperationsList<FileEntry>(){
            {"Force Delete", Delete.ForceDeleteFile, Keys.Shift | Keys.Delete}
        };

        public static OperationsList<List<FileEntry>> MultipleFilesOperations = new OperationsList<List<FileEntry>>(){
            {"Export files...", Export.ExportFiles},
            {"Delete", Delete.AskDeleteFiles, Keys.Delete}
        };

        public static OperationsList<List<FileEntry>> ShortcutsMultipleFilesOperations = new OperationsList<List<FileEntry>>(){
            {"Force Delete", Delete.ForceDeleteFiles, Keys.Shift | Keys.Delete}
        };

        public static OperationsList<DirectoryEntry> FolderOperations = new OperationsList<DirectoryEntry>(){
            {"Export folder...", Export.ExportFolder},
            {"Delete", Delete.AskDeleteFolder, Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
            {"Rename", Rename.RenameFolder, Keys.F2, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static OperationsList<DirectoryEntry> ShortcutsFolderOperations = new OperationsList<DirectoryEntry>(){
            {"Force Delete", Delete.ForceDeleteFolder, Keys.Shift | Keys.Delete, false, delegate(DirectoryEntry entry) { return !entry.IsRoot(); }},
        };

        public static void PerformDefaultAction<T>(OperationsList<T> operations, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj) && operation.IsDefault)
                {
                    operation.Operation(obj);
                    return;
                }
            }
        }

        public static void PopulateContextMenu<T>(OperationsList<T> operations, ContextMenuStrip contextMenu, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj))
                {
                    var currentOperation = operation;
                    contextMenu.Items.Add(currentOperation.Text, null, new EventHandler(delegate(Object o, EventArgs a)
                    {
                        currentOperation.Operation(obj);
                    }));
                }
            }
        }

        public static void PerformActionByKey<T>(OperationsList<T> operations, OperationsList<T> shortcutOperations, Keys key, T obj)
        {
            foreach (var operation in operations)
            {
                if (operation.CheckCondition(obj) && operation.KeyboardShortcut != Keys.None && operation.KeyboardShortcut == key)
                {
                    operation.Operation(obj);
                    return;
                }
            }
            foreach (var operation in shortcutOperations)
            {
                if (operation.CheckCondition(obj) && operation.KeyboardShortcut == key)
                {
                    operation.Operation(obj);
                    return;
                }
            }
        }
    }
}
