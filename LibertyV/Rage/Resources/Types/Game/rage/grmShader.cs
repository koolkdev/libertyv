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
    class grmShader : ClassTypeInfo // Madeup name
    {
        public static grmShader TypeInfo;
        public static void Initialize() { TypeInfo = new grmShader(); }
        protected grmShader()
            : base("rage::grmShader")
        {
            // The data contains parameters list from the type rage::grmShaderParameter, and the data for rage::grmShaderCoordinatesParameter of them, and the list of name hashes.
            AddMember("ParametersData", "PointerReader"); // TODO: change to pointer?
            AddMember("UnknownHash1", "Dword");
            AddMember("ParametersCount", "Byte");
            AddMember("Unknown1", "Byte"); // Bucket?
            AddMember("Unknown2", "Byte"); // PhyMaterial?
            AddMember("Unknown3", "Byte");
            AddMember("ParametersNameHashesOffset", "Word");
            AddMember("ParametersSize", "Word");
            AddMember("UnknownHash2", "Dword");
            AddMember("Unknown4", "Word");
            AddMember("Unknown5", "Byte");
            AddMember("Unknown6", "Byte");
            AddMember("Unknown7", "Word");
            AddMember("Unknown8", "Byte");
            AddMember("TexturesCount", "Byte"); // For ref count usage?
            // Just so we can set them later
            AddMember("Parameters", "None"); // grmShaderParameter[]
            AddMember("ParametersNameHashes", "None"); // Dword[]
        }

        public override ResourceObject Create(ResourceReader reader)
        {
            ResourceObject res = base.Create(reader);
            reader = res["ParametersData"].Value as ResourceReader;
            res["Parameters"] = Basic.Array.GetArrayTypeInfo(Game.rage.grmShaderParameter.TypeInfo, res["ParametersCount"].IntegerValue).Create(reader.Clone());
            reader.Skip(res["ParametersNameHashesOffset"].IntegerValue);
            res["ParametersNameHashes"] = Basic.Array.GetArrayTypeInfo(Basic.Dword.TypeInfo, res["ParametersCount"].IntegerValue).Create(reader.Clone());

            return res;
        }
    }
}
