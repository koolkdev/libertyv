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
        private static Dictionary<string, Func<TypeInfo, TypeInfo>> Templates = new Dictionary<string, Func<TypeInfo, TypeInfo>>();

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

            PointerReader.Initialize();

            // Basic not independent blocks first
            Basic.None.Initialize();
            Basic.Byte.Initialize();
            Basic.Word.Initialize();
            Basic.Dword.Initialize();
            Basic.Float.Initialize();
            Basic.CString.Initialize();
            Basic.VoidPointer.Initialize();

            // 
            Basic.Vector3.Initialize();
            Basic.Vector4.Initialize();
            Basic.MemoryInfo.Initialize();
            Game.rage.datBase.Initialize();
            Game.rage.pgBase.Initialize();
            Game.rage.grcTexture.Initialize();
            Game.rage.grcTextureReferenceBase.Initialize();
            Game.rage.grcTextureReference.Initialize();
            Game.D3DBaseTexture.Initialize();
            Game.rage.grcTextureXenon.Initialize();
            Game.rage.grcTextureGCM.Initialize();

            Game.rage.grmShaderParameter.Initialize();
            Game.rage.grmShaderTextureParameter.Initialize();
            Game.rage.grmShaderCoordinatesParameter.Initialize();
            Game.rage.grmShader.Initialize();
            Game.rage.grmShaderGroup.Initialize();
            Game.rage.rmcDrawableBase.Initialize();
            Game.rage.rmcDrawable.Initialize();
        }

        public static TypeInfo GetTypeInfoByName(string name)
        {
            TypeInfo type;
            if (!TypeInfo.TypesInfo.TryGetValue(name, out type))
            {
                if (name.EndsWith("*"))
                {
                    // Check if pointer.
                    type = GetTypeInfoByName(name.Substring(0, name.Length - 1));
                    if (type == null)
                    {
                        return null;
                    }
                    return Pointer.GetPointerTypeInfo(type);
                }
                else if (name.EndsWith(">"))
                {
                    if (name.IndexOf("<") == -1)
                    {
                        return null;
                    }
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
                else if (name.EndsWith("]"))
                {
                    if (name.IndexOf("[") == -1)
                    {
                        return null;
                    }
                    string objectType = name.Substring(0, name.IndexOf("["));
                    type = GetTypeInfoByName(objectType);
                    if (type == null)
                    {
                        return null;
                    }
                    int count;
                    if (!int.TryParse(name.Substring(name.IndexOf("[") + 1, name.Length - objectType.Length - 2), out count))
                    {
                        return null;
                    }
                    return Basic.Array.GetArrayTypeInfo(type, count);
                }
                return null;
            }
            return type;
        }
    }
}
