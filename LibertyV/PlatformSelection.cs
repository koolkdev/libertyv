using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibertyV
{
    public partial class PlatformSelection : Form
    {
        public PlatformSelection()
        {
            InitializeComponent();
        }

        private void xboxSelect_Click(object sender, EventArgs e)
        {
            GlobalOptions.Platform = GlobalOptions.PlatformType.XBOX360;
            Close();
        }

        private void playstationSelect_Click(object sender, EventArgs e)
        {
            GlobalOptions.Platform = GlobalOptions.PlatformType.PLAYSTATION3;
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
