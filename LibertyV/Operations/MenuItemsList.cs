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
    class MenuItemsList<T> : List<IMenuItem<T>>, IMenuItem<T>
    {
        public void Add(string text, Action<T> operationFunction, Keys keyboardShortcut = Keys.None, bool isDefault = false, Func<T, bool> conditionFunction = null, bool hideIfDisabled = true)
        {
            if (conditionFunction == null)
            {
                conditionFunction = delegate(T obj) { return true; };
            }
            Add(new OperationMenuItem<T>(text, operationFunction, keyboardShortcut, isDefault, conditionFunction, hideIfDisabled));
        }

        public void Add(string name, MenuItemsList<T> submenu)
        {
            Add(new SubMenuItem<T>(name, submenu));
        }

        public void Add(Operations.SeperatorClass obj)
        {
            Add(new SeperatorMenuItem<T>());
        }

        public ToolStripItem[] GetContextMenuItems(T obj)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();
            foreach (IMenuItem<T> item in this)
            {
                items.AddRange(item.GetContextMenuItems(obj));
            }
            return items.ToArray();
        }

        public Action<T> GetDefaultAction(T obj)
        {
            foreach (IMenuItem<T> item in this)
            {
                Action<T> res = item.GetDefaultAction(obj);
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }

        public Action<T> GetActionByKey(Keys key, T obj)
        {
            foreach (IMenuItem<T> item in this)
            {
                Action<T> res = item.GetActionByKey(key, obj);
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }
    }
}
