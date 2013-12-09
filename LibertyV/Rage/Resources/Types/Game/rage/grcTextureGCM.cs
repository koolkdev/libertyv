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
            : base("rage::grcTextureGCM", rage.grcTexture.TypeInfo)
        {
            AddMember("Unknown1", Basic.UInteger32.TypeInfo);
            AddMember("Unknown2", Basic.UInteger32.TypeInfo);
            AddMember("Width", Basic.UInteger16.TypeInfo);
            AddMember("Height", Basic.UInteger16.TypeInfo);
            AddMember("Unknown3", Basic.UInteger32.TypeInfo);
            AddMember("Unknonw4", Basic.UInteger32.TypeInfo);
            AddMember("DataOffset", Basic.UInteger32.TypeInfo);
            AddMember("Name", PointerTypeInfo.GetPointerTypeInfo(Basic.CString.TypeInfo));
            AddMember("Unknown5", Basic.UInteger32.TypeInfo);
            AddMember("PointerToUnknown1", Basic.VoidPointer.TypeInfo);
            AddMember("Unknown6", Basic.UInteger32.TypeInfo);
            AddMember("Unknown7", Basic.UInteger32.TypeInfo);
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
