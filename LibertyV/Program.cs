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

            new PlatformSelection().ShowDialog();

            if (GlobalOptions.Platform == GlobalOptions.PlatformType.NONE) {
                return;
            }

            if (!File.Exists("key.dat"))
            {
                MessageBox.Show("Couldn't find key.dat");
                return;
            }

            AES.Key = File.ReadAllBytes("key.dat");

            string keyMD5 = BitConverter.ToString(new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(AES.Key)).Replace("-", "");

            // check if md5 of key is one of the known keys
            if (GlobalOptions.Platform == GlobalOptions.PlatformType.XBOX360)
            {
                if (keyMD5 != "ead1ea1a3870557b424bc8cf73f51018".ToUpper())
                {
                    MessageBox.Show("Invalid key for Xbox 360.");
                    return;
                }
            }

            if (GlobalOptions.Platform == GlobalOptions.PlatformType.PLAYSTATION3)
            {
                if (keyMD5 != "1df41d237d8056ec87a5bc71925c4cde".ToUpper())
                {
                    MessageBox.Show("Invalid key for Playstation 3.");
                    return;
                }
            }
            Application.Run(new LibertyV());
        }
    }
}
