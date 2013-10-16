using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LibertyV.Operations
{
    class SubMenuItem<T> : IMenuItem<T>
    {
        public SubMenuItem(string text, MenuItemsList<T> list)
        {
            this.Text = text;
            this.List = list;
        }

        private string Text;
        private MenuItemsList<T> List;

        public ToolStripItem[] GetContextMenuItems(T obj)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.DropDownItems.AddRange(List.GetContextMenuItems(obj));
            return new ToolStripItem[] { item };
        }


        public Action<T> GetDefaultAction(T obj)
        {
            return List.GetDefaultAction(obj);
        }

        public Action<T> GetActionByKey(Keys key, T obj)
        {
            return List.GetActionByKey(key, obj);
        }
    }
}
