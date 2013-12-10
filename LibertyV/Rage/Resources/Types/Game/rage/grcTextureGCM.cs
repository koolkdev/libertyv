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
    class grcTextureGCM : InheritanceClassTypeInfo
    {
        // TODO
        public static grcTextureGCM TypeInfo = new grcTextureGCM();
        protected grcTextureGCM()
            : base("rage::grcTextureGCM", "rage::grcTexture")
        {
            AddMember("Unknown1", "UInteger32");
            AddMember("Unknown2", "UInteger32");
            AddMember("Width", "UInteger16");
            AddMember("Height", "UInteger16");
            AddMember("Unknown3", "UInteger32");
            AddMember("Unknonw4", "UInteger32");
            AddMember("DataOffset", "UInteger32");
            AddMember("Name", "CString*");
            AddMember("Unknown5", "UInteger32");
            AddMember("PointerToUnknown1", "void*");
            AddMember("Unknown6", "UInteger32");
            AddMember("Unknown7", "UInteger32");
            // TODO: Figure out this object
            AddMember("UnknownObject", PointerTypeInfo.GetPointerTypeInfo(UnknownObject.TypeInfo));
        }

        private class UnknownObject : ClassTypeInfo
        {
            public static UnknownObject TypeInfo = new UnknownObject();
            protected UnknownObject()
                : base("rage::grcTextureGCM::UnknownObject")
            {
                AddMember("Unknown1", Basic.UInteger32.TypeInfo);
                AddMember("Unknown2", Basic.UInteger32.TypeInfo);
            }
        }
    }
}
