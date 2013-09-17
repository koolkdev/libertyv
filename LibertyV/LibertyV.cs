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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RPF7Viewer.RPF7;
using RPF7Viewer.RPF7.Entries;
using System.IO;
using LibertyV.Utils;

namespace RPF7Viewer
{
    public partial class LibertyV : Form
    {
        public RPF7File File = null;
        public LibertyV()
        {
            InitializeComponent();
            filesList.Columns.Add("Name", 300);
            filesList.Columns.Add("Size", 100);
            filesList.Columns.Add("Resource Type", 100);
        }

        private void LoadRPF(RPF7File rpf)
        {
            this.File = rpf;

            exportAllButton.Enabled = true;
            UpdateExportSelectButton();
            filesTree.Nodes.Clear();
            filesList.Items.Clear();
            TreeNode root = (rpf.Root as DirectoryEntry).GetTreeNodes();
            root.Text = rpf.Filename;
            filesTree.Nodes.Add(root);
        }

        private void fileOpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RAGE Package Format|*.rpf";
            openFileDialog.Title = "Select a file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadRPF(new RPF7File(openFileDialog.OpenFile(), Path.GetFileName(openFileDialog.FileName)));
            }
        }

        private void filesTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateExportSelectButton();
            filesList.Items.Clear();

            foreach (ListViewItem item in (e.Node as EntryTreeNode).Entry.GetListViewItems())
            {
                filesList.Items.Add(item);
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

        private void exportSelectedButton_Click(object sender, EventArgs e)
        {
            ExtractSelected();
        }

        private void UpdateExportSelectButton()
        {
            exportSelectedButton.Enabled = (filesList.SelectedItems.Count > 0 && !filesTree.Focused) || (filesTree.SelectedNode != null && filesTree.Focused);
        }
        
        private void filesList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_Leave(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTree_Enter(object sender, EventArgs e)
        {
            UpdateExportSelectButton();
        }

        private void filesTreeContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            extractButtonFilesTreeMenu.Enabled = filesTree.SelectedNode != null;
            deleteButtonFilesTreeMenu.Enabled = false;
        }

        private void filesTree_MouseDown(object sender, MouseEventArgs e)
        {
            filesTree.SelectedNode = filesTree.GetNodeAt(e.X, e.Y);
            UpdateExportSelectButton();
        }

        private void extractButtonFilesTreeMenu_Click(object sender, EventArgs e)
        {
            ExtractSelected();
        }

        private void filesListContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            extractButtonFilesListMenu.Enabled = filesList.SelectedItems.Count > 0;
            deleteButtonFilesListMenu.Enabled = false;
        }

        private void extractButtonFilesListMenu_Click(object sender, EventArgs e)
        {
            ExtractSelected();
        }


    }
}
