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
    class Array : ResourceObject
    {
        private class ArrayTypeInfo : TypeInfo
        {
            private TypeInfo ArrayItemType;
            private int ArraySize;

            // TODO: I would like that the name will be T**
            public ArrayTypeInfo(TypeInfo typeInfo, int arraySize)
                : base(String.Format("{0}[{1}]", typeInfo.Name, arraySize))
            {
                ArrayItemType = typeInfo;
                ArraySize = arraySize;
            }

            public override ResourceObject Create()
            {
                throw new NotImplementedException();
            }

            public override ResourceObject Create(ResourceReader reader)
            {
                ResourceObject [] objects = new ResourceObject[ArraySize];
                for (int i = 0; i < ArraySize; ++i) {
                    objects[i] = ArrayItemType.Create(reader);
                }
                return new Array(ArrayItemType, objects);
            }

        }

        private static Dictionary<Tuple<TypeInfo, int>, ArrayTypeInfo> Cache = new Dictionary<Tuple<TypeInfo, int>, ArrayTypeInfo>();

        public static TypeInfo GetArrayTypeInfo(TypeInfo arrayItem, int itemsCount)
        {
            ArrayTypeInfo res;
            if (!Cache.TryGetValue(Tuple.Create(arrayItem, itemsCount), out res))
            {
                res = new ArrayTypeInfo(arrayItem, itemsCount);
                Cache[Tuple.Create(arrayItem, itemsCount)] = res;
            }
            return res;
        }

        private ResourceObject[] Items = null;
        private TypeInfo ItemsType;
        public Array(TypeInfo type, ResourceObject[] items)
        {
            Type = GetArrayTypeInfo(type, items.Length);
            ItemsType = type;
            Items = items;
        }

        public override ResourceObject this[object key]
        {
            get
            {
                if (!(key is int))
                {
                    throw new ArgumentException();
                }
                // This operation will throw the appropiate exception if needed
                return Items[(int)key];
            }

            set
            {
                if (!(key is int))
                {
                    throw new ArgumentException();
                }
                // TODO: Check object
                Items[(int)key] = value;
            }
        }

        public override object Value
        {
            get { throw new NotImplementedException(); }
        }

        public override Tuple<string, ResourceObject>[] GetChilds()
        {
            return Items.Select((item, index) => Tuple.Create(String.Format("[{0}]", index), item)).ToArray();
        }
    }
}
