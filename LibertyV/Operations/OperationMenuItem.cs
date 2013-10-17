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
    class OperationMenuItem<T> : IMenuItem<T>
    {
        public OperationMenuItem(string text, Action<T> operationFunction, Keys keyboardShortcut, bool isDefault, Func<T, bool> conditionFunction, bool hideIfDisabled)
        {
            this.Text = text;
            this.Operation = operationFunction;
            this.KeyboardShortcut = keyboardShortcut;
            this.IsDefault = isDefault;
            this.CheckCondition = conditionFunction;
            this.HideIfDisabled = hideIfDisabled;
        }

        private string Text;
        private Action<T> Operation;
        private Keys KeyboardShortcut;
        private bool IsDefault;
        private Func<T, bool> CheckCondition;
        private bool HideIfDisabled;

        public ToolStripItem[] GetContextMenuItems(T obj)
        {
            bool cond = CheckCondition(obj);
            if (cond || !HideIfDisabled)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(Text, null, new EventHandler(delegate(Object o, EventArgs a)
                {
                    Operation(obj);
                }), KeyboardShortcut);
                if (!cond) {
                    item.Enabled = false;
                }
                return new ToolStripItem[] { item };
            }
            return new ToolStripItem[] { };
        }

        public Action<T> GetDefaultAction(T obj)
        {
            if (CheckCondition(obj) && IsDefault)
            {
                return Operation;
            }
            return null;
        }

        public Action<T> GetActionByKey(Keys key, T obj)
        {
            if (CheckCondition(obj) && KeyboardShortcut != Keys.None && KeyboardShortcut == key)
            {
                return Operation;
            }
            return null;
        }
    }
}
