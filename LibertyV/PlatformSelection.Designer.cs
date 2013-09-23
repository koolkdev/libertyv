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

namespace LibertyV
{
    partial class PlatformSelection
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
            this.xboxSelect = new System.Windows.Forms.Button();
            this.playstationSelect = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // xboxSelect
            // 
            this.xboxSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.xboxSelect.Location = new System.Drawing.Point(12, 12);
            this.xboxSelect.Name = "xboxSelect";
            this.xboxSelect.Size = new System.Drawing.Size(150, 50);
            this.xboxSelect.TabIndex = 0;
            this.xboxSelect.Text = "Xbox 360";
            this.xboxSelect.UseVisualStyleBackColor = true;
            this.xboxSelect.Click += new System.EventHandler(this.xboxSelect_Click);
            // 
            // playstationSelect
            // 
            this.playstationSelect.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.playstationSelect.Location = new System.Drawing.Point(12, 68);
            this.playstationSelect.Name = "playstationSelect";
            this.playstationSelect.Size = new System.Drawing.Size(150, 50);
            this.playstationSelect.TabIndex = 1;
            this.playstationSelect.Text = "PlayStation 3";
            this.playstationSelect.UseVisualStyleBackColor = true;
            this.playstationSelect.Click += new System.EventHandler(this.playstationSelect_Click);
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(93, 128);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(69, 23);
            this.closeButton.TabIndex = 2;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(12, 128);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(69, 23);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.Text = "Settings";
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // PlatformSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 163);
            this.ControlBox = false;
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.playstationSelect);
            this.Controls.Add(this.xboxSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlatformSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Version Selection";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button xboxSelect;
        private System.Windows.Forms.Button playstationSelect;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button settingsButton;
    }
}