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

namespace LibertyV.Operations
{
    partial class ImportingWindow
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.filesDataView = new System.Windows.Forms.DataGridView();
            this.filenameCulomn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operationColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.typeColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.compressedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.encryptedColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.isRSC7Column = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filesDataView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(465, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(149, 113);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resources";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Enabled = false;
            this.radioButton3.Location = new System.Drawing.Point(9, 84);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(47, 17);
            this.radioButton3.TabIndex = 3;
            this.radioButton3.Text = "Raw";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Enabled = false;
            this.radioButton2.Location = new System.Drawing.Point(9, 61);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(84, 17);
            this.radioButton2.TabIndex = 2;
            this.radioButton2.Text = ".sys and .gfx";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(9, 38);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(53, 17);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "RSC7";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Resources are:";
            // 
            // importButton
            // 
            this.importButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.importButton.Location = new System.Drawing.Point(465, 357);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(149, 77);
            this.importButton.TabIndex = 2;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.importButton_Click);
            // 
            // filesDataView
            // 
            this.filesDataView.AllowUserToAddRows = false;
            this.filesDataView.AllowUserToDeleteRows = false;
            this.filesDataView.AllowUserToResizeRows = false;
            this.filesDataView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.filesDataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.filesDataView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.filenameCulomn,
            this.operationColumn,
            this.typeColumn,
            this.compressedColumn,
            this.encryptedColumn,
            this.isRSC7Column});
            this.filesDataView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.filesDataView.Location = new System.Drawing.Point(12, 13);
            this.filesDataView.MultiSelect = false;
            this.filesDataView.Name = "filesDataView";
            this.filesDataView.RowHeadersVisible = false;
            this.filesDataView.Size = new System.Drawing.Size(447, 421);
            this.filesDataView.TabIndex = 3;
            // 
            // filenameCulomn
            // 
            this.filenameCulomn.HeaderText = "Filename";
            this.filenameCulomn.Name = "filenameCulomn";
            this.filenameCulomn.ReadOnly = true;
            this.filenameCulomn.Width = 219;
            // 
            // operationColumn
            // 
            this.operationColumn.HeaderText = "Operation";
            this.operationColumn.Name = "operationColumn";
            this.operationColumn.Width = 75;
            // 
            // typeColumn
            // 
            this.typeColumn.HeaderText = "Type";
            this.typeColumn.Name = "typeColumn";
            this.typeColumn.Width = 75;
            // 
            // compressedColumn
            // 
            this.compressedColumn.HeaderText = "Compressed";
            this.compressedColumn.Name = "compressedColumn";
            this.compressedColumn.Width = 75;
            // 
            // encryptedColumn
            // 
            this.encryptedColumn.HeaderText = "Encrypted";
            this.encryptedColumn.Name = "encryptedColumn";
            this.encryptedColumn.Visible = false;
            this.encryptedColumn.Width = 75;
            // 
            // isRSC7Column
            // 
            this.isRSC7Column.HeaderText = "IsRSC7";
            this.isRSC7Column.Name = "isRSC7Column";
            this.isRSC7Column.ReadOnly = true;
            this.isRSC7Column.Visible = false;
            // 
            // ImportingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 446);
            this.Controls.Add(this.filesDataView);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportingWindow";
            this.ShowIcon = false;
            this.Text = "Import...";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportingWindow_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filesDataView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.DataGridView filesDataView;
        private System.Windows.Forms.DataGridViewTextBoxColumn filenameCulomn;
        private System.Windows.Forms.DataGridViewComboBoxColumn operationColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn typeColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn compressedColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn encryptedColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn isRSC7Column;
    }
}