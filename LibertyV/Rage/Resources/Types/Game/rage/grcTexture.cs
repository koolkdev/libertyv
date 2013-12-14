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

namespace LibertyV.Rage.Resources.Types.Game.rage
{
    class grcTexture : InheritanceClassTypeInfo
    {
        public static grcTexture TypeInfo;
        public static void Initialize() { TypeInfo = new grcTexture(); }
        protected grcTexture()
            : base("rage::grcTexture", "rage::pgBase")
        {
            AddMember("Unknown1", "Byte");
            AddMember("Unknown2", "Byte");
            AddMember("Unknown3", "Byte");
            AddMember("Unknown4", "Byte");
            AddMember("Unknown5", "Dword");
            AddMember("Width", "Word");
            AddMember("Height", "Word");
            AddMember("Unknown6", "Dword");
            AddMember("Unknonw7", "Dword");
            AddMember("DataOffset", "Dword");
            AddMember("Name", "CString*");
            AddMember("UseCount", "Word");
            AddMember("ObjectType", "Byte"); // Bitfield TODO. 2 least bits 1 - texture, 2 - texture reference
            AddMember("Unknown8", "Byte");
            AddMember("PointerToUnknown1", "void*");
            AddMember("Unknown9", "Byte");
            AddMember("Unknown10", "Byte");
            AddMember("Unknown11", "Byte");
            AddMember("Unknown12", "Byte");
            AddMember("Unknown13", "Dword");
        }

        public override ResourceObject Create(ResourceReader reader)
        {
            ResourceObject obj = base.Create(reader.Clone());
            switch (((Byte)obj["ObjectType"].Value & 3))
            {
                case 0:
                    switch (GlobalOptions.Platform)
                    {
                        case Platform.PlatformType.XBOX360:
                            return Game.rage.grcTextureXenon.TypeInfo.Create(reader);
                        case Platform.PlatformType.PLAYSTATION3:
                            return Game.rage.grcTextureGCM.TypeInfo.Create(reader);
                        default:
                            throw new Exception("Invalid platform");
                    }
                case 2:
                    return Game.rage.grcTextureReference.TypeInfo.Create(reader);
                default:
                    throw new Exception("Unexpected texture ObjectType");
            }
        }
    }
}
