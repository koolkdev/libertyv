using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LibertyV.Rage.RPF.V7;
using System.IO;
using LibertyV.Rage;
using LibertyV.Utils;

namespace LibertyV.Operations
{
    static class MainFileOperations
    {
        public static bool CanSave(LibertyV window)
        {
            return window.File != null;
        }

        public static bool CanOpen(LibertyV window)
        {
            return window.TempOutputFile == null;
        }

        public static void Open(LibertyV window)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "RAGE Package Format|*.rpf";
            openFileDialog.Title = "Select a file";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                RPF7File file = null;
                window.CurrentFilePath = openFileDialog.FileName;
                Stream stream = openFileDialog.OpenFile();
                try
                {
                    new ProgressWindow("Open", progress =>
                    {
                        progress.SetMessage("Loading..");
                        progress.SetProgress(-1);
                        file = new RPF7File(stream, Path.GetFileName(openFileDialog.FileName));
                    }).Run();
                }
                catch (RPFParsingException ex)
                {
                    MessageBox.Show(ex.Message, "Failed to load RPF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    stream.Close();
                    return;
                }
                window.LoadRPF(file);
            }
        }

        public static void Save(LibertyV window)
        {

            if (MessageBox.Show("Are you sure you want to save all changes?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }
            string originalFilename = window.File.Filename;

            // Write to temporary file
            string tempFilePath = null;
            FileStream file = null;
            if (window.CurrentFilePath != null)
            {
                tempFilePath = window.CurrentFilePath + "." + Path.GetRandomFileName();
                file = System.IO.File.Create(tempFilePath);
            }
            else
            {
                // This is a temporary file, we need to open it with the right flags
                tempFilePath = window.TempOutputFile;
                file = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            }

            ProgressWindow progress = new ProgressWindow("Saving", update => window.File.Write(file, update), true);
            try
            {
                progress.Run();
            }
            catch (OperationCanceledException)
            {
                // This operation cancelled    
                if (window.CurrentFilePath != null)
                {
                    // delete the file
                    file.Close();
                    System.IO.File.Delete(tempFilePath);
                }
                else
                {
                    // Well it is a temporary file, so we don't want to delete it, just make it empty again
                    file.SetLength(0);
                    file.Close();
                }
                MessageBox.Show("Operation canceled.");
                return;
            }

            file.SetLength(file.Position);

            if (window.CurrentFilePath != null)
            {
                // Update the file and reopen it
                file.Close();
                window.CloseRPF(false);
                System.IO.File.Delete(window.CurrentFilePath);
                System.IO.File.Move(tempFilePath, window.CurrentFilePath);

                // Now load the file
                file = System.IO.File.Open(window.CurrentFilePath, FileMode.Open);
                window.LoadRPF(new RPF7File(file, originalFilename), true);
            }
            else
            {
                // Notice: We are not going to use the file that we just wrote, because if we are going to write it again, we will read and write from the same file
                // We waste little bit resources right now (because we doesn't release the old resource because we aren't using the new one), but it isn't so bad
                file.Close();
            }
        }

        public static void SaveAs(LibertyV window)
        {
            string result = GUI.FileSaveSelection(Path.GetFileName(window.File.Filename));
            if (result == null)
            {
                return;
            }
            FileStream file = null;
            try
            {
                file = System.IO.File.Create(result);
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Failed to open file for writing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProgressWindow progress = new ProgressWindow("Saving", update => window.File.Write(file, update), true);
            try
            {
                progress.Run();
            }
            catch (OperationCanceledException)
            {
                // delete the file
                file.Close();
                System.IO.File.Delete(result);
                MessageBox.Show("Operation canceled.");
                return;
            }

            // Now open this file
            file.Seek(0, SeekOrigin.Begin);
            window.CurrentFilePath = result;
            window.LoadRPF(new RPF7File(file, Path.GetFileName(result)), true);
        }

        public static void Close(LibertyV window)
        {
            window.CloseRPF();
        }

        public static void Exit(LibertyV window)
        {
            window.Close();
        }
    }
}
