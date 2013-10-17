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
