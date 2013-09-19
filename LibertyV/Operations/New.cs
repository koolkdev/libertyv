using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibertyV.RPF7.Entries;

namespace LibertyV.Operations
{
    static class New
    {
        public static void NewFolder(DirectoryEntry entry)
        {
            string name = "New Folder";
            int i = 1;
            for (; entry.Entries.Any(e => e.Name == name); ++i, name = String.Format("New Folder ({0})", i));
            DirectoryEntry addedFolder = new DirectoryEntry(name, new List<Entry>());
            entry.Entries.Add(addedFolder);
            addedFolder.Parent = entry;
            entry.Node.Nodes.Add(new EntryTreeNode(addedFolder, new EntryTreeNode[]{}));
            if (!entry.Node.IsExpanded)
            {
                entry.Node.Expand();
            }
            addedFolder.Node.TreeView.SelectedNode = addedFolder.Node;
            addedFolder.Node.TreeView.LabelEdit = true;
            addedFolder.Node.BeginEdit();
        }
    }
}
