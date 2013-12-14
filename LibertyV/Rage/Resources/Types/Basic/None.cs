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
    class None : ResourceObject
    {
        private class NoneTypeInfo : TypeInfo
        {
            public NoneTypeInfo()
                : base("None")
            {
            }

            public override ResourceObject Create()
            {
                throw new NotImplementedException();
            }

            public override ResourceObject Create(ResourceReader reader)
            {
                return new None();
            }
        }

        public static TypeInfo TypeInfo;
        public static void Initialize() { TypeInfo = new NoneTypeInfo(); }

        public None()
        {
            this.Type = TypeInfo;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
