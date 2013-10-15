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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibertyV.Rage.RPF.V7;
using LibertyV.Rage.RPF.V7.Entries;
using System.IO;
using LibertyV.Utils;
using LibertyV.Rage;

namespace LibertyV
{
    public partial class LibertyV : Form
    {
        private class NodeSorter : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                return (x as TreeNode).Text.CompareTo((y as TreeNode).Text);
            }
        }

        public RPF7File File = null;
        private string TempOutputFile = null;
        private string CurrentFilePath = null;

        public LibertyV(RPF7File rpf = null, string tempOutputFile = null)
        {
            InitializeComponent();

            TempOutputFile = tempOutputFile;

            filesList.Columns.Add("Name", 300);
            filesList.Columns.Add("Size", 100);
            filesList.Columns.Add("Resource Type", 100);

            Comparison<TreeNode> treeSorter = (x, y) => x.Name.CompareTo(y.Name);
            filesTree.TreeViewNodeSorter = new NodeSorter();

            if (rpf != null)
            {
                this.LoadRPF(rpf);
            }

            if (tempOutputFile != null)
            {
                // limit the gui,, TODO: better
                fileOpenButton.Enabled = false;
            }
        }

        private void UpdateGUIEntiresNodes(EntryTreeNode node, string path)
        {
            node.Entry = this.File.Root.GetEntry(path) as DirectoryEntry;
            if (node.Entry == null)
            {
                throw new Exception("Failed to update GUI with new file");
            }
            node.Entry.Node = node;
            foreach (TreeNode cnode in node.Nodes)
            {
                UpdateGUIEntiresNodes(cnode as EntryTreeNode, path + "/" + cnode.Text);
            }
        }
        private void UpdateGUIEntries()
        {
            // Update tree
            UpdateGUIEntiresNodes(filesTree.Nodes[0] as EntryTreeNode, "");
            filesTree.Nodes[0].Text = this.File.Filename;

            foreach (TreeNode node in filesTree.Nodes)
            {
                EntryTreeNode enode = node as EntryTreeNode;
                if (enode.Parent == null)
                {
                    // root entry
                    enode.Entry = this.File.Root;
                    this.File.Root.Node = enode;
                }
                else
                {
                    string path = enode.Text;
                    EntryTreeNode currentNode = enode;
                    while ((currentNode = currentNode.Parent as EntryTreeNode).Parent != null)
                    {
                        path = currentNode.Text + "/" + path;
                    }
                }
            }

            if (filesTree.SelectedNode != null)
            {
                EntryTreeNode enode = filesTree.SelectedNode as EntryTreeNode;

                // Update list
                foreach (ListViewItem item in filesList.Items)
                {
                    EntryListViewItem eitem = item as EntryListViewItem;

                    eitem.Entry = enode.Entry.GetEntry(eitem.Text) as FileEntry;
                    if (eitem.Entry == null)
                    {
                        throw new Exception("Failed to update GUI with new file");
                    }
                    eitem.Entry.ViewItem = eitem;
                }
            }
        }

        private void LoadRPF(RPF7File rpf, bool updateGui = false)
        {
            // Close file
            CloseRPF(!updateGui);
            this.File = rpf;
            this.Text = String.Format("Liberty V - {0}", this.File.Filename);

            this.saveAsButton.Enabled = true;
            this.saveButton.Enabled = true;

            if (updateGui)
            {
                UpdateGUIEntries();
            }
            else
            {
                exportAllButton.Enabled = true;
                UpdateExportSelectButton();
                filesTree.Nodes.Clear();
                filesList.Items.Clear();

                TreeNode root = GetTreeNodes(this.File.Root as DirectoryEntry);
                root.Text = rpf.Filename;
                filesTree.Nodes.Add(root);

                filesTree.SelectedNode = root;
                UpdateExportSelectButton();
                UpdateFilesList();
            }
        }

        public void CloseRPF(bool clearGui = true)
        {
            if (this.File != null)
            {
                // Close last file
                this.File.Close();
                this.File = null;
            }
            this.saveAsButton.Enabled = false;
            this.saveButton.Enabled = false;

            if (clearGui)
            {
                filesTree.Nodes.Clear();
                filesList.Items.Clear();
                this.Text = String.Format("Liberty V - Grand Theft Auto V RPF Viewer");
            }
        }

        public static EntryTreeNode GetTreeNodes(DirectoryEntry entry)
        {
            List<EntryTreeNode> children = new List<EntryTreeNode>();
            foreach (Entry childEntry in entry.GetEntries())
            {
                if (childEntry is DirectoryEntry)
                {
                    children.Add(GetTreeNodes(childEntry as DirectoryEntry));
                }
            }
            return new EntryTreeNode(entry, children.ToArray());
        }

        private DirectoryEntry LastSelectedEntry = null;
        private void UpdateFilesList(bool clear = true)
        {
            DirectoryEntry dirEntry = (filesTree.SelectedNode == null) ? null : (filesTree.SelectedNode as EntryTreeNode).Entry;
            if (LastSelectedEntry != dirEntry)
            {
                if (LastSelectedEntry != null)
                {
                    LastSelectedEntry.FilesListView = filesList;
                }
                if (dirEntry != null)
                {
                    dirEntry.FilesListView = filesList;
                }
                LastSelectedEntry = dirEntry;
            }
            if (clear)
            {
                filesList.Items.Clear();

                if (filesTree.SelectedNode != null)
                {
                    foreach (Entry entry in (filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries())
                    {
                        if (entry is FileEntry)
                        {
                            filesList.Items.Add(new EntryListViewItem(entry as FileEntry));
                        }
                    }
                }
            }
            else if (filesTree.SelectedNode == null)
            {
                filesList.Items.Clear();
            }
            else
            {
                HashSet<string> seenItems = new HashSet<string>();
                // find the changes
                foreach (EntryListViewItem item in filesList.Items)
                {
                    if (!(filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries().Any(entry => entry == item.Entry))
                    {
                        filesList.Items.Remove(item);
                    }
                    else
                    {
                        item.Update();
                        seenItems.Add(item.Entry.Name);
                    }
                }
                foreach (Entry entry in (filesTree.SelectedNode as EntryTreeNode).Entry.GetEntries())
                {
                    if (entry is FileEntry)
                    {
                        if (!seenItems.Any(name => name == entry.Name))
                        {
                            filesList.Items.Add(new EntryListViewItem(entry as FileEntry));
                        }
                    }
                }
            }
        }

        private void ExtractSelected()
        {
            string selectedFolder = GUI.FolderSelection();
            if (selectedFolder != null)
            {
                if (!filesTree.Focused)
                {
                    foreach (EntryListViewItem entry in filesList.SelectedItems)
                    {
                        entry.Entry.Export(selectedFolder);
                    }
                }
                if (filesTree.SelectedNode != null && filesTree.Focused)
                {
                    (filesTree.SelectedNode as EntryTreeNode).Entry.Export(selectedFolder);

                }
            }
        }

        private void UpdateExportSelectButton()
        {
            exportSelectedButton.Enabled = (filesList.SelectedItems.Count > 0 && !filesTree.Focused) || (filesTree.SelectedNode != null && filesTree.Focused);
        }


        #region Menu buttons

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RAGE Package Format|*.rpf";
            openFileDialog.Title = "Select a file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                RPF7File file = null;
                CurrentFilePath = openFileDialog.FileName;
                Stream stream = openFileDialog.OpenFile();
                try
                {
                    new ProgressWindow("Open", progress =>
                    {
                        progress.SetMessage("Loading..");
                        progress.SetProgress(-1);
                        file = new RPF7File(stream, Path.GetFileName(openFileDialog.FileName));
                    }).Run();
                }
                catch (RPFParsingException ex)
                {
                    MessageBox.Show(ex.Message, "Failed to load RPF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    stream.Close();
                    return;
                }
                LoadRPF(file);
            }
        }

        private void exportAllButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderBrowserDialog = new FolderSelectDialog();
            if (folderBrowserDialog.ShowDialog())
            {
                (File.Root as DirectoryEntry).Export(folderBrowserDialog.FileName);
            }
        }

        private void exportSelectedButton_Click(object sender, EventArgs e)
        {
            ExtractSelected();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save all changes?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            string originalFilename = this.File.Filename;

            // Write to temporary file
            string tempFilePath = null;
            FileStream file = null;
            if (CurrentFilePath != null)
            {
                tempFilePath = CurrentFilePath + "." + Path.GetRandomFileName();
                file = System.IO.File.Create(tempFilePath);
            }
            else
            {
                // This is a temporary file, we need to open it with the right flags
                tempFilePath = TempOutputFile;
                file = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            }

            ProgressWindow progress = new ProgressWindow("Saving", update => this.File.Write(file, update), true);
            try
            {
                progress.Run();
            }
            catch (OperationCanceledException)
            {
                // This operation cancelled    
                if (CurrentFilePath != null)
                {
                    // delete the file
                    file.Close();
                    System.IO.File.Delete(tempFilePath);
                }
                else
                {
                    // Well it is a temporary file, so we don't want to delete it, just make it empty again
                    file.SetLength(0);
                    file.Close();
                }
                MessageBox.Show("Operation canceled.");
                return;
            }

            file.SetLength(file.Position);

            if (CurrentFilePath != null)
            {
                // Update the file and reopen it
                file.Close();
                CloseRPF(false);
                System.IO.File.Delete(CurrentFilePath);
                System.IO.File.Move(tempFilePath, CurrentFilePath);
                file = System.IO.File.Open(CurrentFilePath, FileMode.Open);
            }

            // Now open this file
            file.Seek(0, SeekOrigin.Begin);
            LoadRPF(new RPF7File(file, originalFilename), true);
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            string result = GUI.FileSaveSelection(Path.GetFileName(this.File.Filename));
            if (result == null)
            {
                return;
            }
            FileStream file = null;
            try
            {
                file = System.IO.File.Create(result);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Failed to open file for writing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProgressWindow progress = new ProgressWindow("Saving", update => this.File.Write(file, update), true);
            try
            {
                progress.Run();
            }
            catch (OperationCanceledException)
            {
                // delete the file
                file.Close();
                System.IO.File.Delete(result);
                MessageBox.Show("Operation canceled.");
                return;
            }

            // Now open this file
            file.Seek(0, SeekOrigin.Begin);
            CurrentFilePath = result;
            LoadRPF(new RPF7File(file, Path.GetFileName(result)), true);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            new Settings.Settings().ShowDialog();
        }

        #endregion

        #region Files list

        private void filesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateExportSelectButton();
        }

        #endregion

        #region Files tree

        private void filesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateFilesList();
        }

        private void filesTree_Leave(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_Enter(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_MouseDown(object sender, MouseEventArgs e)
        {
            filesTree.SelectedNode = filesTree.GetNodeAt(e.X, e.Y);
            UpdateExportSelectButton();
            UpdateFilesList();
        }

        #endregion

        #region Files tree context menu

        private void filesTreeContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.filesTreeContextMenuStrip.Items.Clear();
        }

        private void filesTreeContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.filesTreeContextMenuStrip.Items.Clear();
            if (filesTree.SelectedNode != null)
            {
                Operations.Operations.PopulateContextMenu(Operations.Operations.FolderOperations, this.filesTreeContextMenuStrip, (filesTree.SelectedNode as EntryTreeNode).Entry);
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void filesTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (filesTree.SelectedNode != null)
            {
                Operations.Operations.PerformActionByKey(Operations.Operations.FolderOperations, Operations.Operations.ShortcutsFolderOperations, e.KeyData, (filesTree.SelectedNode as EntryTreeNode).Entry);
            }
        }

        private void filesTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            filesTree.LabelEdit = false;
            EntryTreeNode entryItem = e.Node as EntryTreeNode;
            // copy-paste sadly
            if (e.Label == null || e.Label == "")
            {
                // invalid name, don't create message box
                e.CancelEdit = true;
                //entryItem.Update();
            }
            else if (e.Label == entryItem.Entry.Name)
            {
                // do nothing
            }
            else if (entryItem.Entry.Parent.GetEntries().Any(entry => entry.Name == e.Label))
            {
                MessageBox.Show("Name already used.");
                e.CancelEdit = true;
            }
            else
            {
                entryItem.Entry.Name = e.Label;
            }
            if (!e.CancelEdit)
            {
                e.Node.Text = e.Label;
                filesTree.Sort();
                e.CancelEdit = true;
            }
        }

        #endregion

        #region Files list context menu


        private void filesListContextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this.filesListContextMenuStrip.Items.Clear();
        }

        private void filesListContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.filesListContextMenuStrip.Items.Clear();
            if (filesList.SelectedItems.Count == 0)
            {
                if (filesTree.SelectedNode == null)
                {
                    e.Cancel = true;
                    return;
                }
                Operations.Operations.PopulateContextMenu(Operations.Operations.FilesListOperations, this.filesListContextMenuStrip, (filesTree.SelectedNode as EntryTreeNode).Entry);
            }
            else if (filesList.SelectedItems.Count == 1)
            {
                Operations.Operations.PopulateContextMenu(Operations.Operations.FileOperations, this.filesListContextMenuStrip, (filesList.SelectedItems[0] as EntryListViewItem).Entry);
            }
            else
            {
                List<FileEntry> entries = new List<FileEntry>();
                foreach (EntryListViewItem entry in filesList.SelectedItems)
                {
                    entries.Add(entry.Entry);
                }
                Operations.Operations.PopulateContextMenu(Operations.Operations.MultipleFilesOperations, this.filesListContextMenuStrip, entries);
            }
            e.Cancel = false;
        }

        private void filesList_KeyDown(object sender, KeyEventArgs e)
        {
            if (filesList.SelectedItems.Count == 0)
            {
                if (filesTree.SelectedNode != null)
                {
                    Operations.Operations.PerformActionByKey(Operations.Operations.FilesListOperations, Operations.Operations.ShortcutsFilesListOperations, e.KeyData, (filesTree.SelectedNode as EntryTreeNode).Entry);
                }
            }
            else if (filesList.SelectedItems.Count == 1)
            {
                FileEntry entry = (filesList.SelectedItems[0] as EntryListViewItem).Entry;
                if (e.KeyData == Keys.Enter)
                {
                    Operations.Operations.PerformDefaultAction(Operations.Operations.FileOperations, entry);
                }
                else
                {
                    Operations.Operations.PerformActionByKey(Operations.Operations.FileOperations, Operations.Operations.ShortcutsFileOperations, e.KeyData, entry);
                }
            }
            else
            {
                List<FileEntry> entries = new List<FileEntry>();
                foreach (EntryListViewItem entry in filesList.SelectedItems)
                {
                    entries.Add(entry.Entry);
                }

                Operations.Operations.PerformActionByKey(Operations.Operations.MultipleFilesOperations, Operations.Operations.ShortcutsMultipleFilesOperations, e.KeyData, entries);
            }
        }

        private void filesList_DoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ListViewItem item = filesList.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    Operations.Operations.PerformDefaultAction(Operations.Operations.FileOperations, (item as EntryListViewItem).Entry);
                }
            }
        }

        private void filesList_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            filesList.LabelEdit = false;
            EntryListViewItem entryItem = (filesList.Items[e.Item] as EntryListViewItem);
            if (e.Label == null || e.Label == "")
            {
                // invalid name, don't create message box
                e.CancelEdit = true;
                //entryItem.Update();
            }
            else if (e.Label == entryItem.Entry.Name)
            {
                // do nothing
            }
            else if (entryItem.Entry.Parent.GetEntries().Any(entry => entry.Name == e.Label))
            {
                MessageBox.Show("Name already used.");
                e.CancelEdit = true;
            }
            else
            {
                entryItem.Entry.Name = e.Label;
            }
            if (!e.CancelEdit)
            {
                entryItem.Update();
                filesList.Sort();
                e.CancelEdit = true;
            }
        }

        #endregion

        private void LibertyV_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.File != null)
            {
                this.File.Close();
                this.File = null;
            }
        }
    }
}
