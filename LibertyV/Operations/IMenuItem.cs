using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    interface IMenuItem<T>
    {
        ToolStripItem[] GetContextMenuItems(T obj);
        Action<T> GetDefaultAction(T obj);
        Action<T> GetActionByKey(Keys key, T obj);
    }
}
