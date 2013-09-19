using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibertyV.RPF7.Entries;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    static class Helper
    {
        static public void SelectAll(DirectoryEntry entry)
        {
            if (entry.FilesListView != null)
            {
                foreach (ListViewItem item in entry.FilesListView.Items)
                {
                    item.Selected = true;
                }
            }
        }
        static public void SelectAll(FileEntry entry)
        {
            SelectAll(entry.Parent);
        }
        static public void SelectAll(List<FileEntry> entries)
        {
            SelectAll(entries[0]);
        }
    }
}
