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
    }
}
