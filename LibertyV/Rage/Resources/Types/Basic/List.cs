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
    class List : TemplateClassTypeInfo
    {
        private class ListItems : ResourceObject
        {
            private ResourceObject[] Items = null;

            private ResourceReader Reader = null;

            private TypeInfo ItemsType;

            public ListItems(TypeInfo type, ResourceReader reader)
            {
                Type = ListItemsTypeInfo.GetTemplateClassTypeInfo<ListItemsTypeInfo>(type);
                ItemsType = type;
                Reader = reader;
            }

            public void InitializeWithReader(int count, int size)
            {
                // Reader should not be null
                Items = new ResourceObject[size];
                int i = 0;
                for (; i < count; ++i)
                {
                    Items[i] = ItemsType.Create(Reader);
                }
                for (;  i < size; ++i)
                {
                    Items[i] = ItemsType.Create();
                }
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

        private class ListItemsTypeInfo : TemplateClassTypeInfo
        {
            // TODO: I would like that the name will be T**
            protected ListItemsTypeInfo(TypeInfo typeInfo)
                : base("ListItems", typeInfo)
            {
            }

            public override ResourceObject Create()
            {
                throw new NotImplementedException();
            }

            public override ResourceObject Create(ResourceReader reader)
            {
                return new ListItems(base.TemplateItem, reader.Clone());
            }
        }
        protected List(TypeInfo templateItem)
            : base("List", templateItem)
        {
            base.AddMember("Items", PointerTypeInfo.GetPointerTypeInfo(ListItemsTypeInfo.GetTemplateClassTypeInfo<ListItemsTypeInfo>(base.TemplateItem)));
            base.AddMember("Count", Basic.UInteger16.TypeInfo);
            base.AddMember("Size", Basic.UInteger16.TypeInfo);
        }

        public override ResourceObject Create()
        {
            throw new NotImplementedException();
        }

        public ResourceObject Create(List<ResourceObject> objects)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override ResourceObject Create(ResourceReader reader)
        {
            ResourceObject obj = base.Create(reader);
            // Load the items list
            ((ListItems)obj["Items"].Value).InitializeWithReader((int)(UInt16)obj["Count"].Value, (int)(UInt16)obj["Size"].Value);
            return obj;
        }
    }
}
