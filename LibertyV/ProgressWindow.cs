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
using LibertyV.Utils;
using System.Threading;

namespace LibertyV
{
    public partial class ProgressWindow : Form, IProgressReport
    {
        public const int ProgressBarUnits = 1000;
        private string message;
        private int maxProgress = ProgressWindow.ProgressBarUnits;
        private int lastProgress = 0;
        private int lastReportedProgress = 0;
        private string title;
        private bool unknownProgress = false;
        private bool lastUnknownProgress = false;

        private Exception error = null;
        private AutoResetEvent completeEvent = new AutoResetEvent(false);

        public delegate void ProgressWork(IProgressReport update);

        ProgressWork Worker;

        public ProgressWindow(string title, ProgressWork worker, bool cancelSupport = false)
        {
            InitializeComponent();

            this.title = title;
            this.Text = String.Format("{0} - {1}%", title, lastProgress * 100 / maxProgress);

            this.label.Text = "";

            cancelButton.Enabled = cancelSupport;
            backgroundWorker.WorkerSupportsCancellation = cancelSupport;

            Worker = worker;
        }

        public void Run()
        {
            backgroundWorker.RunWorkerAsync();
            this.ShowDialog();
            this.completeEvent.WaitOne();
            if (error != null)
            {
                throw error;
            }
        }


        public void SetProgress(long value)
        {
            if (value == -1)
            {
                if (!lastUnknownProgress)
                {
                    this.unknownProgress = true;
                    backgroundWorker.ReportProgress(this.lastProgress);
                }
            }
            else
            {
                value = value * ProgressWindow.ProgressBarUnits / maxProgress;
                this.unknownProgress = false;
                this.lastReportedProgress = (int)value;
                if (lastUnknownProgress || value != this.lastProgress || message != null)
                {
                    backgroundWorker.ReportProgress((int)value);
                }
            }
        }

        public void SetMessage(string message)
        {
            this.message = message;
            backgroundWorker.ReportProgress(this.lastReportedProgress);
        }

        public bool IsCanceled()
        {
            return backgroundWorker.CancellationPending;
        }

        public long GetFullValue()
        {
            return maxProgress;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Worker(this);
            }
            catch (OperationCanceledException)
            {
                e.Cancel = true;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Close when complete
            Close();
            if (e.Cancelled)
            {
                error = new OperationCanceledException();
            }
            else
            {
                error = e.Error;
            }
            this.completeEvent.Set();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (unknownProgress)
            {
                if (!lastUnknownProgress)
                {
                    lastUnknownProgress = true;
                    progressBar.Style = ProgressBarStyle.Marquee;
                    this.Text = this.title;
                }
            }
            else
            {
                if (lastUnknownProgress || this.lastProgress != e.ProgressPercentage)
                {
                    if (lastUnknownProgress)
                    {
                        lastUnknownProgress = false;
                        progressBar.Style = ProgressBarStyle.Blocks;
                    }
                    this.lastProgress = e.ProgressPercentage;
                    progressBar.Value = e.ProgressPercentage;
                    this.Text = String.Format("{0} - {1}%", this.title, this.lastProgress * 100 / this.maxProgress);
                }
            }
            if (message != null)
            {
                label.Text = message;
                // Don't update text next time
                message = null;
            }
        }
    }
}
