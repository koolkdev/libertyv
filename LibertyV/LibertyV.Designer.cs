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

namespace RPF7Viewer
{
    partial class LibertyV
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LibertyV));
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.fileOpenButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exportAllButton = new System.Windows.Forms.ToolStripButton();
            this.exportSelectedButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.filesTree = new System.Windows.Forms.TreeView();
            this.filesTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractButtonFilesTreeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteButtonFilesTreeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.filesList = new System.Windows.Forms.ListView();
            this.filesListContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.extractButtonFilesListMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteButtonFilesListMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.filesTreeContextMenuStrip.SuspendLayout();
            this.filesListContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbar
            // 
            this.toolbar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileOpenButton,
            this.toolStripSeparator1,
            this.exportAllButton,
            this.exportSelectedButton});
            this.toolbar.Location = new System.Drawing.Point(0, 0);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(776, 39);
            this.toolbar.TabIndex = 2;
            this.toolbar.Text = "toolbar";
            // 
            // fileOpenButton
            // 
            this.fileOpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fileOpenButton.Image = global::LibertyV.Properties.Resources.open;
            this.fileOpenButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileOpenButton.Name = "fileOpenButton";
            this.fileOpenButton.Size = new System.Drawing.Size(36, 36);
            this.fileOpenButton.Text = "fileOpenButton";
            this.fileOpenButton.Click += new System.EventHandler(this.fileOpenButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // exportAllButton
            // 
            this.exportAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exportAllButton.Enabled = false;
            this.exportAllButton.Image = ((System.Drawing.Image)(resources.GetObject("exportAllButton.Image")));
            this.exportAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportAllButton.Name = "exportAllButton";
            this.exportAllButton.Size = new System.Drawing.Size(61, 36);
            this.exportAllButton.Text = "Export All";
            this.exportAllButton.Click += new System.EventHandler(this.exportAllButton_Click);
            // 
            // exportSelectedButton
            // 
            this.exportSelectedButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exportSelectedButton.Enabled = false;
            this.exportSelectedButton.Image = ((System.Drawing.Image)(resources.GetObject("exportSelectedButton.Image")));
            this.exportSelectedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportSelectedButton.Name = "exportSelectedButton";
            this.exportSelectedButton.Size = new System.Drawing.Size(91, 36);
            this.exportSelectedButton.Text = "Export Selected";
            this.exportSelectedButton.Click += new System.EventHandler(this.exportSelectedButton_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 39);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.filesTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.filesList);
            this.splitContainer1.Size = new System.Drawing.Size(776, 452);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 3;
            // 
            // filesTree
            // 
            this.filesTree.ContextMenuStrip = this.filesTreeContextMenuStrip;
            this.filesTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesTree.Location = new System.Drawing.Point(0, 0);
            this.filesTree.Name = "filesTree";
            this.filesTree.Size = new System.Drawing.Size(200, 452);
            this.filesTree.TabIndex = 1;
            this.filesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.filesTree_AfterSelect);
            this.filesTree.Enter += new System.EventHandler(this.filesTree_Enter);
            this.filesTree.Leave += new System.EventHandler(this.filesTree_Leave);
            this.filesTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.filesTree_MouseDown);
            // 
            // filesTreeContextMenuStrip
            // 
            this.filesTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractButtonFilesTreeMenu,
            this.deleteButtonFilesTreeMenu});
            this.filesTreeContextMenuStrip.Name = "filesTreeContextMenuStrip";
            this.filesTreeContextMenuStrip.Size = new System.Drawing.Size(155, 48);
            this.filesTreeContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.filesTreeContextMenuStrip_Opening);
            // 
            // extractButtonFilesTreeMenu
            // 
            this.extractButtonFilesTreeMenu.Name = "extractButtonFilesTreeMenu";
            this.extractButtonFilesTreeMenu.Size = new System.Drawing.Size(154, 22);
            this.extractButtonFilesTreeMenu.Text = "Extract Folder...";
            this.extractButtonFilesTreeMenu.Click += new System.EventHandler(this.extractButtonFilesTreeMenu_Click);
            // 
            // deleteButtonFilesTreeMenu
            // 
            this.deleteButtonFilesTreeMenu.Name = "deleteButtonFilesTreeMenu";
            this.deleteButtonFilesTreeMenu.Size = new System.Drawing.Size(154, 22);
            this.deleteButtonFilesTreeMenu.Text = "Delete";
            // 
            // filesList
            // 
            this.filesList.ContextMenuStrip = this.filesListContextMenuStrip;
            this.filesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filesList.Location = new System.Drawing.Point(0, 0);
            this.filesList.Name = "filesList";
            this.filesList.Size = new System.Drawing.Size(572, 452);
            this.filesList.TabIndex = 2;
            this.filesList.UseCompatibleStateImageBehavior = false;
            this.filesList.View = System.Windows.Forms.View.Details;
            this.filesList.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.filesList_ItemSelectionChanged);
            // 
            // filesListContextMenuStrip
            // 
            this.filesListContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractButtonFilesListMenu,
            this.deleteButtonFilesListMenu});
            this.filesListContextMenuStrip.Name = "filesListContextMenuStrip";
            this.filesListContextMenuStrip.Size = new System.Drawing.Size(145, 48);
            this.filesListContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.filesListContextMenuStrip_Opening);
            // 
            // extractButtonFilesListMenu
            // 
            this.extractButtonFilesListMenu.Name = "extractButtonFilesListMenu";
            this.extractButtonFilesListMenu.Size = new System.Drawing.Size(144, 22);
            this.extractButtonFilesListMenu.Text = "Extract Files...";
            this.extractButtonFilesListMenu.Click += new System.EventHandler(this.extractButtonFilesListMenu_Click);
            // 
            // deleteButtonFilesListMenu
            // 
            this.deleteButtonFilesListMenu.Name = "deleteButtonFilesListMenu";
            this.deleteButtonFilesListMenu.Size = new System.Drawing.Size(144, 22);
            this.deleteButtonFilesListMenu.Text = "Delete";
            // 
            // LibertyV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 491);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolbar);
            this.Name = "LibertyV";
            this.Text = "LibertyV - Grand Theft Auto V RPF Explorer";
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.filesTreeContextMenuStrip.ResumeLayout(false);
            this.filesListContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.ToolStripButton fileOpenButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton exportAllButton;
        private System.Windows.Forms.ToolStripButton exportSelectedButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView filesTree;
        private System.Windows.Forms.ListView filesList;
        private System.Windows.Forms.ContextMenuStrip filesTreeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem extractButtonFilesTreeMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteButtonFilesTreeMenu;
        private System.Windows.Forms.ContextMenuStrip filesListContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem extractButtonFilesListMenu;
        private System.Windows.Forms.ToolStripMenuItem deleteButtonFilesListMenu;
    }
}

