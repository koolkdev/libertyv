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

        public ToolStripItem GetContextMenuItemUpdatedOnOpening(T obj)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(Text);
            item.DropDownOpening += (o, e) =>
            {
                item.DropDownItems.Clear();
                item.DropDownItems.AddRange(List.GetContextMenuItems(obj));
            };
            return item;
        }
    }
}
