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
    class TypesCache
    {
        private static Dictionary<string, TypeInfo> Cache = new Dictionary<string, TypeInfo>();
        private static Dictionary<string, Func<TypeInfo, TypeInfo>> Templates = new Dictionary<string, Func<TypeInfo, TypeInfo>>();

        private static void AddToCache(TypeInfo type)
        {
            Cache[type.Name] = type;
        }

        private static void RegisterTemplate(string name, Func<TypeInfo, TypeInfo> func)
        {
            Templates[name] = func;
        }

        public static void Initialize()
        {
            // Register templates
            RegisterTemplate("List", Basic.List.GetTemplateClassTypeInfo<Basic.List>);
            RegisterTemplate("rage::pgDictionary", Game.rage.pgDictionary.GetTemplateClassTypeInfo<Game.rage.pgDictionary>);

            // Make sure that all the types are initialized in the right order

            // Basic not independent blocks first
            AddToCache(Basic.UInteger16.TypeInfo);
            AddToCache(Basic.UInteger32.TypeInfo);
            AddToCache(Basic.CString.TypeInfo);
            AddToCache(Basic.VoidPointer.TypeInfo);

            // 
            AddToCache(Basic.MemoryInfo.TypeInfo);
            AddToCache(Game.rage.pgBase.TypeInfo);
            AddToCache(Game.rage.grcTexture.TypeInfo);
            AddToCache(Game.D3DBaseTexture.TypeInfo);
            AddToCache(Game.rage.grcTextureXenon.TypeInfo);
            AddToCache(Game.rage.grcTextureGCM.TypeInfo);
        }

        public static TypeInfo GetTypeInfoByName(string name)
        {
            TypeInfo type;
            // Some of the members wlil be only registered in TypeInfo
            if (!Cache.TryGetValue(name, out type) && !TypeInfo.TypesInfo.TryGetValue(name, out type))
            {
                if (name.EndsWith("*"))
                {
                    // Check if pointer.
                    type = GetTypeInfoByName(name.Substring(0, name.Length - 1));
                    if (type == null)
                    {
                        return null;
                    }
                    return PointerTypeInfo.GetPointerTypeInfo(type);
                }
                else if (name.EndsWith(">"))
                {
                    // Is a template
                    string templateName = name.Substring(0, name.IndexOf("<"));
                    if (!Templates.ContainsKey(templateName))
                    {
                        return null;
                    }
                    string typename = name.Substring(name.IndexOf("<") + 1, name.Length - templateName.Length - 2);
                    type = GetTypeInfoByName(typename);
                    if (type == null)
                    {
                        return null;
                    }
                    return Templates[templateName](type);
                }
                return null;
            }
            return type;
        }
    }
}
