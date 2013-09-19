/*
 
    RPF7Viewer - Viewer for RAGE Package File version 7
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
    class OperationInfo<T>
    {
        public OperationInfo(string text, Action<T> operationFunction, Keys keyboardShortcut, bool isDefault, Func<T, bool> conditionFunction)
        {
            this._text = text;
            this._operation = operationFunction;
            this._keyboardShortcut = keyboardShortcut;
            this._isDefault = isDefault;
            this._checkCondition = conditionFunction;
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
        }

        private Action<T> _operation;
        public Action<T> Operation
        {
            get
            {
                return _operation;
            }
        }

        private Keys _keyboardShortcut;
        public Keys KeyboardShortcut
        {
            get
            {
                return _keyboardShortcut;
            }
        }

        private bool _isDefault;
        public bool IsDefault
        {
            get
            {
                return _isDefault;
            }
        }

        private Func<T, bool> _checkCondition;
        public Func<T, bool> CheckCondition
        {
            get
            {
                return _checkCondition;
            }
        }
    }

    class OperationsList<T> : List<OperationInfo<T>>
    {
        public void Add(string text, Action<T> operationFunction, Keys keyboardShortcut = Keys.None, bool isDefault = false, Func<T, bool> conditionFunction = null)
        {
            if (conditionFunction == null)
            {
                conditionFunction = delegate(T obj) { return true; };
            }
            Add(new OperationInfo<T>(text, operationFunction, keyboardShortcut, isDefault, conditionFunction));
        }
    }
}
