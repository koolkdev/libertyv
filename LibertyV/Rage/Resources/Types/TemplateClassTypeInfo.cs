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
using System.Reflection;

namespace LibertyV.Rage.Resources.Types
{
    abstract class TemplateClassTypeInfo : ClassTypeInfo
    {
        protected TypeInfo TemplateItem;

        // TODO: Support more than one template item
        protected TemplateClassTypeInfo(string name, TypeInfo templateItem)
            : base(name + "<" + templateItem.Name + ">")
        {
            this.TemplateItem = templateItem;
        }

        protected TemplateClassTypeInfo(TypeInfo templateItem)
            : base("")
        {
            throw new NotImplementedException();
        }

        private class Cache<T> where T : TemplateClassTypeInfo
        {
            public static Dictionary<TypeInfo, T> CacheItems = new Dictionary<TypeInfo, T>();
        }

        public static T GetTemplateClassTypeInfo<T>(TypeInfo templateItem) where T : TemplateClassTypeInfo
        {
            T res;
            if (!Cache<T>.CacheItems.TryGetValue(templateItem, out res))
            {
                res = (T)Activator.CreateInstance(typeof(T), BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { templateItem }, null);
                Cache<T>.CacheItems[templateItem] = res;
            }
            return res;
        }

    }
}
