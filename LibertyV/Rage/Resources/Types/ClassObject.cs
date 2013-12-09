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
    class ClassObject : ResourceObject
    {
        private ClassTypeInfo Info;

        private Dictionary<String, ResourceObject> MembersValues = new Dictionary<string, ResourceObject>();

        public ClassObject(ClassTypeInfo type, Dictionary<String, ResourceObject> values = null)
        {
            base.Type = type;
            this.Info = type;
            foreach (var member in this.Info.Members)
            {
                if (values != null)
                {
                    ResourceObject obj;
                    if (values.TryGetValue(member.Item1, out obj))
                    {
                        // TODO: Check if value is valid
                        MembersValues[member.Item1] = obj;
                        continue;
                    }
                }
                MembersValues[member.Item1] = member.Item2.Create();
            }
        }
        public override ResourceObject this[object key]
        {
            get
            {
                ResourceObject obj;
                if (MembersValues.TryGetValue(key.ToString(), out obj))
                {
                    return obj;
                }
                throw new KeyNotFoundException(String.Format("{0} doesn't have the field {1}", this.Info.Name, key.ToString()));
            }
            set
            {
                if (MembersValues.ContainsKey(key.ToString()))
                {
                    // TODO: Check if member is valid
                    MembersValues[key.ToString()] = value;
                }
                throw new KeyNotFoundException(String.Format("{0} doesn't have the field {1}", this.Info.Name, key.ToString()));
            }
        }

        public override object Value
        {
            get
            {
                throw new InvalidOperationException("Class doesn't have value");
            }
        }

        public override Tuple<string, ResourceObject>[] GetChilds()
        {
            return MembersValues.Select(x => Tuple.Create(x.Key, x.Value)).ToArray();
        }
    }
}
