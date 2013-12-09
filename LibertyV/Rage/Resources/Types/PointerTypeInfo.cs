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

namespace LibertyV.Rage.Resources.Types
{
    class PointerTypeInfo : TypeInfo
    {
        private TypeInfo PointedInfo;
        private static Dictionary<TypeInfo, PointerTypeInfo> PointerTypesCache = new Dictionary<TypeInfo, PointerTypeInfo>();

        protected PointerTypeInfo(TypeInfo poinetedInfo)
            : base(poinetedInfo.Name + "*")
        {
            PointedInfo = poinetedInfo;
        }

        override public ResourceObject Create()
        {
            return new Pointer(null, PointedInfo);
        }

        override public ResourceObject Create(ResourceReader reader)
        {
            reader = reader.DereferencePointer();
            if (reader == null)
            {
                return new Pointer(null, PointedInfo);
            }
            return new Pointer(PointedInfo.Create(reader), PointedInfo);
        }

        public static PointerTypeInfo GetPointerTypeInfo(TypeInfo type)
        {
            PointerTypeInfo res;
            if (!PointerTypesCache.TryGetValue(type, out res))
            {
                res = new PointerTypeInfo(type);
                PointerTypesCache[type] = res;
            }
            return res;
        }
    }
}
