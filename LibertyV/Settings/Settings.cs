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
using System.IO;

namespace LibertyV.Settings
{
    enum ExportResourcesChoice
    {
        RSC7,
        SYSGFX,
        RAW
    };

    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();

            LoadSettings();

            applyButton.Enabled = false;
        }

        private void LoadSettings()
        {
            switch (Properties.Settings.Default.ExportResourcesChoice)
            {
                case ExportResourcesChoice.RSC7:
                    {
                        rsc7Button.Checked = true;
                        break;
                    }
                case ExportResourcesChoice.SYSGFX:
                    {
                        sysgfxButton.Checked = true;
                        break;
                    }
                case ExportResourcesChoice.RAW:
                    {
                        rawButton.Checked = true;
                        break;
                    }
                default:
                    {
                        rsc7Button.Checked = true;
                        break;
                    }
            }

            xbox360KeyCheckBox.Checked = Properties.Settings.Default.Xbox360KeyFileEnabled;
            xbox360KeyFile.Enabled = xbox360KeyCheckBox.Checked;
            xbox360KeyFile.Text = Properties.Settings.Default.Xbox360KeyFile;
            ps3KeyCheckBox.Checked = Properties.Settings.Default.PS3KeyFileEnabled;
            ps3KeyFile.Enabled = ps3KeyCheckBox.Checked;
            ps3KeyFile.Text = Properties.Settings.Default.PS3KeyFile;

        }

        private static bool CheckKey(string keyFile, string keymd5)
        {
            if (!File.Exists(keyFile))
            {
                return false;
            }
            byte[] key;
            try
            {
                key = File.ReadAllBytes(keyFile);
            }
            catch (Exception)
            {
                // Failed to read the file
                return false;
            }

            string keyMD5 = BitConverter.ToString(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(key)).Replace("-", "");

            return keyMD5 == keymd5.ToUpper();
        }

        public static bool CheckXbox360Key(string keyFile)
        {
            return CheckKey(keyFile, "ead1ea1a3870557b424bc8cf73f51018");
        }

        public static bool CheckPS3Key(string keyFile)
        {
            return CheckKey(keyFile, "1df41d237d8056ec87a5bc71925c4cde");
        }

        private bool SaveSettings()
        {
            // Verify settings first
            if ((xbox360KeyCheckBox.Checked && !File.Exists(xbox360KeyFile.Text)))
            {
                MessageBox.Show("Invalid key file for Xbox 360.");
                return false;
            }

            if ((ps3KeyCheckBox.Checked && !File.Exists(ps3KeyFile.Text)))
            {
                MessageBox.Show("Invalid key file for PlayStation 3.");
                return false;
            }

            // TODO: If failed to open the file it will save invalid key instead invalid key file..
            if ((xbox360KeyCheckBox.Checked && !CheckXbox360Key(xbox360KeyFile.Text)))
            {
                MessageBox.Show("Invalid key for Xbox 360.");
                return false;
            }

            if ((ps3KeyCheckBox.Checked && !CheckPS3Key(ps3KeyFile.Text)))
            {
                MessageBox.Show("Invalid key for PlayStation 3.");
                return false;
            }

            // Now save them
            if (rsc7Button.Checked)
            {
                Properties.Settings.Default.ExportResourcesChoice = ExportResourcesChoice.RSC7;
            }
            else if (sysgfxButton.Checked)
            {
                Properties.Settings.Default.ExportResourcesChoice = ExportResourcesChoice.SYSGFX;
            }
            else if (rawButton.Checked)
            {
                Properties.Settings.Default.ExportResourcesChoice = ExportResourcesChoice.RAW;
            }

            Properties.Settings.Default.Xbox360KeyFileEnabled = xbox360KeyCheckBox.Checked;
            Properties.Settings.Default.Xbox360KeyFile = xbox360KeyFile.Text;
            Properties.Settings.Default.PS3KeyFileEnabled = ps3KeyCheckBox.Checked;
            Properties.Settings.Default.PS3KeyFile = ps3KeyFile.Text;

            Properties.Settings.Default.Save();

            return true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                Close();
            }
        }


        private void applyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            applyButton.Enabled = false;
        }

        private void SettingsChanged()
        {
            applyButton.Enabled = true;
        }

        private void rsc7Button_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged();
        }

        private void sysgfxButton_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged();
        }

        private void rawButton_CheckedChanged(object sender, EventArgs e)
        {
            SettingsChanged();
        }

        private void xbox360KeyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            xbox360KeyFile.Enabled = xbox360KeyCheckBox.Checked;
            SettingsChanged();
        }

        private void ps3KeyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ps3KeyFile.Enabled = ps3KeyCheckBox.Checked;
            SettingsChanged();
        }

        private void xbox360KeyFile_TextChanged(object sender, EventArgs e)
        {
            SettingsChanged();
        }

        private void ps3KeyFile_TextChanged(object sender, EventArgs e)
        {
            SettingsChanged();
        }
    }
}
