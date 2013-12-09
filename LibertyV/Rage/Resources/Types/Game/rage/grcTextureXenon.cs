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
    class grcTextureXenon : InheritanceClassTypeInfo
    {
        public static grcTextureXenon TypeInfo = new grcTextureXenon();
        protected grcTextureXenon()
            : base("rage::grcTextureXenon", rage.grcTexture.TypeInfo)
        {
            AddMember("Unknown1", Basic.UInteger32.TypeInfo);
            AddMember("Unknown2", Basic.UInteger32.TypeInfo);
            AddMember("Unknown3", Basic.UInteger32.TypeInfo);
            AddMember("Unknown4", Basic.UInteger32.TypeInfo);
            AddMember("Unknown5", Basic.UInteger32.TypeInfo);
            AddMember("Unknonw6", Basic.UInteger32.TypeInfo);
            AddMember("Name", PointerTypeInfo.GetPointerTypeInfo(Basic.CString.TypeInfo));
            AddMember("Unknonw7", Basic.UInteger32.TypeInfo);
            AddMember("Unknonw8", Basic.UInteger32.TypeInfo);
            AddMember("Unknonw9", Basic.UInteger32.TypeInfo);
            AddMember("Unknonw10", Basic.UInteger32.TypeInfo);
            AddMember("TextureInfo", PointerTypeInfo.GetPointerTypeInfo(Game.D3DBaseTexture.TypeInfo));
            AddMember("Width", Basic.UInteger16.TypeInfo);
            AddMember("Height", Basic.UInteger16.TypeInfo);
            AddMember("Unknonw13", Basic.UInteger32.TypeInfo);
        }
    }
}
