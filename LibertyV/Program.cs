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
using System.Linq;
using System.Windows.Forms;
using System.IO;
using LibertyV.Utils;

namespace LibertyV
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check that the configuration is still valid
            if (Properties.Settings.Default.Xbox360KeyFileEnabled && !Settings.Settings.CheckPS3Key(Properties.Settings.Default.Xbox360KeyFile))
            {
                Properties.Settings.Default.Xbox360KeyFileEnabled = false;
            }

            if (Properties.Settings.Default.PS3KeyFileEnabled && !Settings.Settings.CheckPS3Key(Properties.Settings.Default.PS3KeyFile))
            {
                Properties.Settings.Default.PS3KeyFileEnabled = false;
            }

            new PlatformSelection().ShowDialog();

            if (GlobalOptions.Platform == Platform.PlatformType.NONE)
            {
                return;
            }

            Application.Run(new LibertyV());
        }
    }
}
