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
using LibertyV.Rage.RPF.V7.Entries;
using System.IO;
using LibertyV.Rage.RPF.V7;
using LibertyV.Rage.RPF;

namespace LibertyV.Operations
{
    public partial class ImportingWindow : Form
    {
        private string DirectoryPath;
        private DirectoryEntry Entry;
        private Dictionary<string, FileStream> Files = new Dictionary<string, FileStream>();

        string[] UncompressedExtensions = new string[] { ".rpf", ".xpm", ".cpm", ".bik", ".awc" };

        public ImportingWindow(DirectoryEntry addTo, string sourceFolder, List<string> files)
        {
            InitializeComponent();

            this.Entry = addTo;
            this.DirectoryPath = sourceFolder;

            foreach (string filename in files)
            {
                AddFile(filename);
            }
        }

        private bool FileShouldBeCompressed(string filename)
        {
            if (Path.GetFileName(filename) == "introscrn.gfx")
            {
                return false;
            }
            // Check if it has uncompressed extension
            return !UncompressedExtensions.Contains(Path.GetExtension(filename));
        }

        public void AddFile(string filename)
        {
            bool skipFile = false;
            bool isReplace = false;
            bool isRSC7 = false;
            bool isCompressed = true;
            FileStream stream = null;

            // TODO: Add an explnation on why couldn't add file
            // TODO: check all the features here (adding a folder..)
            // Let's try to open the file, we are going to lock the files until the end of the dialog, so thier state won't change
            try
            {
                stream = File.OpenRead(Path.Combine(this.DirectoryPath, filename));
                Files[filename] = stream;
            }
            catch (Exception)
            {
                skipFile = true;
            }

            if (!skipFile)
            {
                try
                {
                    Structs.RSC7Header rsc7Header = new Structs.RSC7Header(stream);

                    // Test the size of the resource
                    if (ResourceEntry.GetSizeFromSystemFlag(rsc7Header.SystemFlag) + ResourceEntry.GetSizeFromGraphicsFlag(rsc7Header.GraphicsFlag) + 0x10 == stream.Length)
                    {
                        isRSC7 = true;
                    }
                }
                catch (Exception)
                {
                    if (stream.Length <= 0x100)
                    {
                        // small files aren't compressed
                        isCompressed = false;
                    }
                    else
                    {
                        isCompressed = FileShouldBeCompressed(filename);
                    }
                }
            }

            if (!skipFile)
            {
                // Check if the directory of the entry is a regular existing file..
                int lastDirSplit = filename.LastIndexOfAny(new char[] { '\\', '/' });
                if (lastDirSplit != -1)
                {
                    string baseFolder = filename.Substring(lastDirSplit);
                    DirectoryEntry lastDirectory = this.Entry;
                    foreach (string directoryPart in baseFolder.Split(new char[] { '\\', '/' }))
                    {
                        if (lastDirectory.GetEntry(directoryPart) == null)
                        {
                            // that is fine, there is no entry like that yet
                            break;
                        }
                        lastDirectory = lastDirectory.GetEntry(directoryPart) as DirectoryEntry;
                        if (lastDirectory == null)
                        {
                            // oh, that is not a directory, that is bad.
                            skipFile = true;
                        }
                    }
                }
            }

            if (!skipFile)
            {
                // Check if there is already entry with that name
                Entry originalEntry = this.Entry.GetEntry(filename);

                // TODO: maybe copy the proporties from the coppied object?
                if (originalEntry != null)
                {
                    if (originalEntry is DirectoryEntry)
                    {
                        skipFile = true;
                    }
                    else
                    {
                        isReplace = true;
                    }
                }


                // Check if file should be compressed
            }

            // let's build the row 
            DataGridViewRow row = new DataGridViewRow();

            var filenameCell = new DataGridViewTextBoxCell();
            var operationCell = new DataGridViewComboBoxCell();
            var typeCell = new DataGridViewComboBoxCell();
            var compressedCell = new DataGridViewCheckBoxCell();
            var encryptedCell = new DataGridViewCheckBoxCell();
            var isRSC7Cell = new DataGridViewCheckBoxCell();

            row.Cells.Add(filenameCell);
            row.Cells.Add(operationCell);
            row.Cells.Add(typeCell);
            row.Cells.Add(compressedCell);
            row.Cells.Add(encryptedCell);
            row.Cells.Add(isRSC7Cell);

            filenameCell.Value = filename;

            if (!skipFile)
            {
                if (isReplace)
                {
                    operationCell.Items.Add("Replace");
                    operationCell.Value = "Replace";
                }
                else
                {
                    operationCell.Items.Add("Add");
                    operationCell.Value = "Add";
                }
            }
            else
            {
                operationCell.Value = "Skip";
                compressedCell.ReadOnly = true;
                encryptedCell.ReadOnly = true;
            }
            operationCell.Items.Add("Skip");

            typeCell.Items.Add("File");

            if (isRSC7)
            {
                typeCell.Items.Add("Resource");
                typeCell.Value = "Resource";
                // TODO: If it is changed to File, it isn't can't be changed..
                compressedCell.Value = true;
                compressedCell.ReadOnly = true;
            }
            else
            {
                typeCell.Value = "File";
                compressedCell.Value = isCompressed;
            }

            isRSC7Cell.Value = isRSC7;

            filesDataView.Rows.Add(row);
        }

        private void ImportingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (FileStream stream in Files.Values)
            {
                stream.Close();
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in filesDataView.Rows)
            {
                string filename = (string)row.Cells[0].Value;
                int lastDirSplit = filename.LastIndexOfAny(new char[] { '\\', '/' });
                string baseFilename = filename.Substring(lastDirSplit + 1);
                string baseFolder = "";

                if (lastDirSplit != -1)
                {
                    baseFolder = filename.Substring(lastDirSplit);
                }

                string operation = (string)row.Cells[1].Value;
                string type = (string)row.Cells[2].Value;
                bool compressed = (bool)row.Cells[3].Value;

                if (operation != "Skip")
                {
                    string filepath = Path.GetTempFileName();
                    // Let's copy the file to temp folder, this file will be deleted on close
                    FileStream writeStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete, 0x1000, FileOptions.DeleteOnClose);
                    Files[filename].Seek(0, SeekOrigin.Begin);
                    Files[filename].CopyTo(writeStream);

                    // Close the stream
                    Files[filename].Close();
                    Files.Remove(filename);

                    // Take an handle to the file, so it will be delete only when this handle will be closed.
                    FileStream readStream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                    writeStream.Close();

                    FileEntry entry;
                    if (type == "File")
                    {
                        entry = new RegularFileEntry(baseFilename, new ExternalFileStreamCreator(readStream), compressed);
                    }
                    else
                    {
                        // We already checked that the file is fine
                        Structs.RSC7Header rsc7header = new Structs.RSC7Header(readStream);
                        // is resouce
                        entry = new ResourceEntry(baseFilename, new RSC7StreamCreator(readStream), rsc7header.SystemFlag, rsc7header.GraphicsFlag);
                    }

                    // Ensure the directory of the file. the file going to have directory in case of importing whole directory tree.
                    if (baseFolder != "")
                    {
                        DirectoryEntry lastDirectory = this.Entry;
                        foreach (string directoryPart in baseFolder.Split(new char[] { '\\', '/' }))
                        {
                            if (lastDirectory.GetEntry(directoryPart) == null)
                            {
                                lastDirectory.AddEntry(new DirectoryEntry(directoryPart, new List<Entry>()));
                            }
                            lastDirectory = lastDirectory.GetEntry(directoryPart) as DirectoryEntry;
                        }
                    }

                    DirectoryEntry realDirectory = ((DirectoryEntry)this.Entry.GetEntry(baseFolder));

                    if (operation == "Replace")
                    {
                        FileEntry oldEntry = this.Entry.GetEntry(filename) as FileEntry;
                        oldEntry.Parent.RemoveEntry(oldEntry);
                    }
                    realDirectory.AddEntry(entry);
                }
            }
            Close();
        }
    }
}
