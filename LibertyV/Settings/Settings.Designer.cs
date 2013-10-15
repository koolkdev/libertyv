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

namespace LibertyV.Settings
{
    partial class Settings
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.generalSettingsPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ps3KeyCheckBox = new System.Windows.Forms.CheckBox();
            this.xbox360KeyCheckBox = new System.Windows.Forms.CheckBox();
            this.ps3KeyFile = new System.Windows.Forms.TextBox();
            this.xbox360KeyFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.exportSettingsPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.sysgfxButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.rawButton = new System.Windows.Forms.RadioButton();
            this.rsc7Button = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.generalSettingsPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.exportSettingsPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalSettingsPage);
            this.tabControl1.Controls.Add(this.exportSettingsPage);
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(323, 363);
            this.tabControl1.TabIndex = 2;
            // 
            // generalSettingsPage
            // 
            this.generalSettingsPage.Controls.Add(this.groupBox2);
            this.generalSettingsPage.Location = new System.Drawing.Point(4, 22);
            this.generalSettingsPage.Name = "generalSettingsPage";
            this.generalSettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalSettingsPage.Size = new System.Drawing.Size(315, 337);
            this.generalSettingsPage.TabIndex = 1;
            this.generalSettingsPage.Text = "General";
            this.generalSettingsPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ps3KeyCheckBox);
            this.groupBox2.Controls.Add(this.xbox360KeyCheckBox);
            this.groupBox2.Controls.Add(this.ps3KeyFile);
            this.groupBox2.Controls.Add(this.xbox360KeyFile);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(303, 114);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Key";
            // 
            // ps3KeyCheckBox
            // 
            this.ps3KeyCheckBox.AutoSize = true;
            this.ps3KeyCheckBox.Location = new System.Drawing.Point(6, 88);
            this.ps3KeyCheckBox.Name = "ps3KeyCheckBox";
            this.ps3KeyCheckBox.Size = new System.Drawing.Size(89, 17);
            this.ps3KeyCheckBox.TabIndex = 9;
            this.ps3KeyCheckBox.Text = "Playstation 3:";
            this.ps3KeyCheckBox.UseVisualStyleBackColor = true;
            this.ps3KeyCheckBox.CheckedChanged += new System.EventHandler(this.ps3KeyCheckBox_CheckedChanged);
            // 
            // xbox360KeyCheckBox
            // 
            this.xbox360KeyCheckBox.AutoSize = true;
            this.xbox360KeyCheckBox.Location = new System.Drawing.Point(6, 65);
            this.xbox360KeyCheckBox.Name = "xbox360KeyCheckBox";
            this.xbox360KeyCheckBox.Size = new System.Drawing.Size(74, 17);
            this.xbox360KeyCheckBox.TabIndex = 8;
            this.xbox360KeyCheckBox.Text = "Xbox 360:";
            this.xbox360KeyCheckBox.UseVisualStyleBackColor = true;
            this.xbox360KeyCheckBox.CheckedChanged += new System.EventHandler(this.xbox360KeyCheckBox_CheckedChanged);
            // 
            // ps3Key
            // 
            this.ps3KeyFile.Location = new System.Drawing.Point(100, 88);
            this.ps3KeyFile.Name = "ps3Key";
            this.ps3KeyFile.Size = new System.Drawing.Size(198, 20);
            this.ps3KeyFile.TabIndex = 7;
            this.ps3KeyFile.TextChanged += new System.EventHandler(this.ps3KeyFile_TextChanged);
            // 
            // xbox360Key
            // 
            this.xbox360KeyFile.Location = new System.Drawing.Point(100, 62);
            this.xbox360KeyFile.Name = "xbox360Key";
            this.xbox360KeyFile.Size = new System.Drawing.Size(197, 20);
            this.xbox360KeyFile.TabIndex = 5;
            this.xbox360KeyFile.TextChanged += new System.EventHandler(this.xbox360KeyFile_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(191, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Configure the key file for each platform.";
            // 
            // exportSettingsPage
            // 
            this.exportSettingsPage.Controls.Add(this.groupBox1);
            this.exportSettingsPage.Location = new System.Drawing.Point(4, 22);
            this.exportSettingsPage.Name = "exportSettingsPage";
            this.exportSettingsPage.Padding = new System.Windows.Forms.Padding(3);
            this.exportSettingsPage.Size = new System.Drawing.Size(315, 337);
            this.exportSettingsPage.TabIndex = 0;
            this.exportSettingsPage.Text = "Export";
            this.exportSettingsPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.sysgfxButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rawButton);
            this.groupBox1.Controls.Add(this.rsc7Button);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(303, 129);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resources";
            // 
            // sysgfxButton
            // 
            this.sysgfxButton.AutoSize = true;
            this.sysgfxButton.Location = new System.Drawing.Point(6, 83);
            this.sysgfxButton.Name = "sysgfxButton";
            this.sysgfxButton.Size = new System.Drawing.Size(84, 17);
            this.sysgfxButton.TabIndex = 4;
            this.sysgfxButton.Text = ".sys and .gfx";
            this.sysgfxButton.UseVisualStyleBackColor = true;
            this.sysgfxButton.CheckedChanged += new System.EventHandler(this.sysgfxButton_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choose how to extract resources.";
            // 
            // rawButton
            // 
            this.rawButton.AutoSize = true;
            this.rawButton.Location = new System.Drawing.Point(6, 106);
            this.rawButton.Name = "rawButton";
            this.rawButton.Size = new System.Drawing.Size(47, 17);
            this.rawButton.TabIndex = 2;
            this.rawButton.Text = "Raw";
            this.rawButton.UseVisualStyleBackColor = true;
            this.rawButton.CheckedChanged += new System.EventHandler(this.rawButton_CheckedChanged);
            // 
            // rsc7Button
            // 
            this.rsc7Button.AutoSize = true;
            this.rsc7Button.Location = new System.Drawing.Point(6, 60);
            this.rsc7Button.Name = "rsc7Button";
            this.rsc7Button.Size = new System.Drawing.Size(53, 17);
            this.rsc7Button.TabIndex = 1;
            this.rsc7Button.Tag = "";
            this.rsc7Button.Text = "RSC7";
            this.rsc7Button.UseVisualStyleBackColor = true;
            this.rsc7Button.CheckedChanged += new System.EventHandler(this.rsc7Button_CheckedChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(164, 377);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(83, 377);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // applyButton
            // 
            this.applyButton.Enabled = false;
            this.applyButton.Location = new System.Drawing.Point(245, 377);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(75, 23);
            this.applyButton.TabIndex = 5;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 412);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.tabControl1.ResumeLayout(false);
            this.generalSettingsPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.exportSettingsPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage exportSettingsPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton sysgfxButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rawButton;
        private System.Windows.Forms.RadioButton rsc7Button;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.TabPage generalSettingsPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ps3KeyCheckBox;
        private System.Windows.Forms.CheckBox xbox360KeyCheckBox;
        private System.Windows.Forms.TextBox ps3KeyFile;
        private System.Windows.Forms.TextBox xbox360KeyFile;

    }
}