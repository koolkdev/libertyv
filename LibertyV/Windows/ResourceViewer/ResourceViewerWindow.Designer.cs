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

namespace LibertyV.Windows.ResourceViewer
{
    partial class ResourceTypeViewer
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
            System.Windows.Forms.ToggleColumnHeader toggleColumnHeader1 = new System.Windows.Forms.ToggleColumnHeader();
            System.Windows.Forms.ToggleColumnHeader toggleColumnHeader2 = new System.Windows.Forms.ToggleColumnHeader();
            System.Windows.Forms.ToggleColumnHeader toggleColumnHeader3 = new System.Windows.Forms.ToggleColumnHeader();
            this.resourcesView = new System.Windows.Forms.TreeListView();
            this.SuspendLayout();
            // 
            // resourcesView
            // 
            this.resourcesView.BackColor = System.Drawing.SystemColors.Window;
            toggleColumnHeader1.Hovered = false;
            toggleColumnHeader1.Image = null;
            toggleColumnHeader1.Index = 0;
            toggleColumnHeader1.Pressed = false;
            toggleColumnHeader1.ScaleStyle = System.Windows.Forms.ColumnScaleStyle.Slide;
            toggleColumnHeader1.Selected = false;
            toggleColumnHeader1.Text = "Name";
            toggleColumnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            toggleColumnHeader1.Visible = true;
            toggleColumnHeader1.Width = 150;
            toggleColumnHeader2.Hovered = false;
            toggleColumnHeader2.Image = null;
            toggleColumnHeader2.Index = 0;
            toggleColumnHeader2.Pressed = false;
            toggleColumnHeader2.ScaleStyle = System.Windows.Forms.ColumnScaleStyle.Slide;
            toggleColumnHeader2.Selected = false;
            toggleColumnHeader2.Text = "Value";
            toggleColumnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            toggleColumnHeader2.Visible = true;
            toggleColumnHeader2.Width = 400;
            toggleColumnHeader3.Hovered = false;
            toggleColumnHeader3.Image = null;
            toggleColumnHeader3.Index = 0;
            toggleColumnHeader3.Pressed = false;
            toggleColumnHeader3.ScaleStyle = System.Windows.Forms.ColumnScaleStyle.Slide;
            toggleColumnHeader3.Selected = false;
            toggleColumnHeader3.Text = "Type";
            toggleColumnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            toggleColumnHeader3.Visible = true;
            toggleColumnHeader3.Width = 200;
            this.resourcesView.Columns.AddRange(new System.Windows.Forms.ToggleColumnHeader[] {
            toggleColumnHeader1,
            toggleColumnHeader2,
            toggleColumnHeader3});
            this.resourcesView.ColumnSortColor = System.Drawing.Color.Gainsboro;
            this.resourcesView.ColumnTrackColor = System.Drawing.Color.WhiteSmoke;
            this.resourcesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resourcesView.GridLineColor = System.Drawing.Color.WhiteSmoke;
            this.resourcesView.HeaderMenu = null;
            this.resourcesView.ItemHeight = 20;
            this.resourcesView.ItemMenu = null;
            this.resourcesView.LabelEdit = false;
            this.resourcesView.Location = new System.Drawing.Point(0, 0);
            this.resourcesView.Name = "resourcesView";
            this.resourcesView.RowSelectColor = System.Drawing.SystemColors.Highlight;
            this.resourcesView.RowTrackColor = System.Drawing.Color.WhiteSmoke;
            this.resourcesView.Size = new System.Drawing.Size(910, 390);
            this.resourcesView.SmallImageList = null;
            this.resourcesView.StateImageList = null;
            this.resourcesView.TabIndex = 0;
            // 
            // ResourceTypeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 390);
            this.Controls.Add(this.resourcesView);
            this.Name = "ResourceTypeViewer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Resource Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeListView resourcesView;




    }
}