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
            AddMember("Unknown1", "UInteger32");
            AddMember("Unknown2", "UInteger32");
            AddMember("Unknown3", "UInteger32");
            AddMember("Unknown4", "UInteger32");
            AddMember("Unknown5", "UInteger32");
            AddMember("Unknown6", "UInteger32");
            AddMember("Unknown7", "UInteger32");
            AddMember("Unknown8", "UInteger32");
            AddMember("Unknown9", "UInteger32");
            AddMember("Unknown10", "UInteger32");
            AddMember("Unknown11", "UInteger32");
            AddMember("Unknown12", "UInteger32");
            AddMember("Unknown13", "UInteger32");
        }
    }
}
