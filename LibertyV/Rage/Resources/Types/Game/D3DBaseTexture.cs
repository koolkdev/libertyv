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

namespace LibertyV.Rage.Resources.Types.Game
{
    class D3DBaseTexture : ClassTypeInfo
    {
        public static D3DBaseTexture TypeInfo = new D3DBaseTexture();
        protected D3DBaseTexture()
            : base("D3DBaseTexture")
        {
            // TODO: Bitfields and enums. Fuck me.
            AddMember("Unknown1", Basic.UInteger32.TypeInfo);
            AddMember("Unknown2", Basic.UInteger32.TypeInfo);
            AddMember("Unknown3", Basic.UInteger32.TypeInfo);
            AddMember("Unknown4", Basic.UInteger32.TypeInfo);
            AddMember("Unknown5", Basic.UInteger32.TypeInfo);
            AddMember("Unknown6", Basic.UInteger32.TypeInfo);
            AddMember("Unknown7", Basic.UInteger32.TypeInfo);
            AddMember("Unknown8", Basic.UInteger32.TypeInfo);
            AddMember("Unknown9", Basic.UInteger32.TypeInfo);
            AddMember("Unknown10", Basic.UInteger32.TypeInfo);
            AddMember("Unknown11", Basic.UInteger32.TypeInfo);
            AddMember("Unknown12", Basic.UInteger32.TypeInfo);
            AddMember("Unknown13", Basic.UInteger32.TypeInfo);
        }
    }
}
