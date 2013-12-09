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

namespace LibertyV.Rage.Resources.Types.Basic
{
    class CString : PrimitiveObject
    {
        private string value;

        private class CStringInfo : PrimitiveTypeInfo
        {
            public CStringInfo()
                : base("CString")
            {
            }

            public override ResourceObject Create()
            {
                return new CString("");
            }

            public override ResourceObject Create(ResourceReader reader)
            {
                string s = "";
                char c = (char)reader.ReadByte();
                while (c != 0) {
                    s += c;
                    c = (char)reader.ReadByte();
                }
                return new CString(s);
            }
        }

        public static PrimitiveTypeInfo TypeInfo = new CStringInfo();

        public CString(string value = "")
        {
            this.value = value;
            this.Type = CString.TypeInfo;
        }

        public override object Value
        {
            get
            {
                return value;
            }
        }
    }
}
