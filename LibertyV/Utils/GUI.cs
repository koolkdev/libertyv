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
using System.Windows.Forms;

namespace LibertyV.Utils
{
    public static class GUI
    {
        public static string FolderSelection(string title = null)
        {

            FolderSelectDialog folderBrowserDialog = new FolderSelectDialog();
            if (folderBrowserDialog.ShowDialog())
            {
                return folderBrowserDialog.FileName;
            }
            return null;
        }
        
        public static string FileSelection(string title = null, string filter = null) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (title != null)
            {
                openFileDialog.Title = title;
            }
            if (filter != null)
            {
                openFileDialog.Filter = filter;
            }
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        public static string FileSaveSelection(string filename = null) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (filename != null)
            {
                saveFileDialog.FileName = filename;
            }
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }
    }
}
