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
            this.exportSettingsPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.exportSettingsPage);
            this.tabControl1.Location = new System.Drawing.Point(5, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(323, 363);
            this.tabControl1.TabIndex = 2;
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
            this.groupBox1.Location = new System.Drawing.Point(3, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 129);
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

    }
}