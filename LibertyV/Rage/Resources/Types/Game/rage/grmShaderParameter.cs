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
    class grmShaderParameter : ClassTypeInfo // Made up name
    {
        public static grmShaderParameter TypeInfo;
        public static void Initialize() { TypeInfo = new grmShaderParameter(); }

        protected grmShaderParameter()
            : base("rage::grmShaderParameter")
        {
            base.AddMember("Count", "Byte");
            base.AddMember("Unknown1", "Byte"); // Register Index?
            base.AddMember("Unknown2", "Byte"); // padding?
            base.AddMember("Unknown3", "Byte");
        }

        public override ResourceObject Create(ResourceReader reader)
        {
            ResourceObject obj = base.Create(reader.Clone());
            if (obj["Count"].IntegerValue == 0)
            {
                return Game.rage.grmShaderTextureParameter.TypeInfo.Create(reader);
            }
            else
            {
                return Game.rage.grmShaderCoordinatesParameter.TypeInfo.Create(reader);
            }
        }
    }
}
