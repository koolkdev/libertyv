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

namespace LibertyV.Utils
{
    class SubProgressReport : IProgressReport
    {
        private float StartProgress;
        private float ProgressLength;
        private long FullValue;
        private float Unit;
        private IProgressReport Parent;

        public SubProgressReport(IProgressReport report, long startProgress, long progress = 1, long units = 100)
        {
            StartProgress = startProgress;
            ProgressLength = progress;
            FullValue = units;

            // No need to while, because the parent of a subprogressreport can't be subprogressreport
            if (report is SubProgressReport)
            {
                StartProgress *= ((SubProgressReport)report).Unit;
                StartProgress += ((SubProgressReport)report).StartProgress;
                ProgressLength *= ((SubProgressReport)report).Unit;
                report = ((SubProgressReport)report).Parent;
            }

            Parent = report;
            Unit = ProgressLength / units;
        }

        public SubProgressReport(IProgressReport report, long units = 100)
            : this(report, 0, report.GetFullValue(), units)
        {
        }

        public void SetProgress(long value)
        {
            Parent.SetProgress((long)(StartProgress + value * Unit));
        }

        public void SetMessage(string message)
        {
            Parent.SetMessage(message);
        }

        public bool IsCanceled()
        {
            return Parent.IsCanceled();
        }

        public long GetFullValue()
        {
            return FullValue;
        }
    }
}
