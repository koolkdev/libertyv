using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    class SeperatorMenuItem<T> : IMenuItem<T>
    {
        public ToolStripItem[] GetContextMenuItems(T obj)
        {
            return new ToolStripItem[] { new ToolStripSeparator() };
        }


        public Action<T> GetDefaultAction(T obj)
        {
            return null;
        }

        public Action<T> GetActionByKey(Keys key, T obj)
        {
            return null;
        }
    }
}
