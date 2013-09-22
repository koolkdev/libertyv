using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        }

        private void SaveSettings()
        {
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
            Properties.Settings.Default.Save();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
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

    }
}
