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
using System.Text;
using System.IO;

namespace LibertyV.Rage.RPF.V7.Entries
{
    public abstract class FileEntry : Entry
    {
        public IStreamCreator Data;
        public EntryListViewItem ViewItem;

        public FileEntry(String filename, IStreamCreator data)
            : base(filename)
        {
            this.Data = data;
        }

        public virtual void Write(Stream stream)
        {
            using (Stream s = this.Data.GetStream()) {
                s.CopyTo(stream);
            }
        }

        public override void Export(String path)
        {
            string filename;
            if (Directory.Exists(path))
            {
                filename = Path.Combine(path, this.Name);
            }
            else
            {
                filename = path;
            }

            using (FileStream file = File.Create(filename))
            {
                using (Stream stream = this.Data.GetStream())
                {
                    stream.CopyTo(file);
                }
            }
        }

        public string GetExtension()
        {
            return Path.GetExtension(Name);
        }

        public bool IsRegularFile()
        {
            return this is RegularFileEntry;
        }

        public bool IsResource()
        {
            return this is ResourceEntry;
        }
    }
}
