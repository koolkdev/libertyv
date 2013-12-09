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
    class Pointer : ResourceObject
    {
        private ResourceObject PointedObject;

        public Pointer(ResourceObject obj, TypeInfo objectType)
        {
            this.PointedObject = obj;
            this.Type = PointerTypeInfo.GetPointerTypeInfo(objectType);
        }

        public Pointer(TypeInfo objectType)
            : this(null, objectType)
        {
        }

        public override ResourceObject this[object key]
        {
            get
            {
                if (PointedObject == null)
                {
                    throw new NullReferenceException("The pointer point to null.");
                }
                // I am not sure that this is always the logical thing to do... (For example what if we have pointer to pointer to pointer..). but for now lets leave it as is.
                return PointedObject[key];
            }
            set
            {
                if (PointedObject == null)
                {
                    throw new NullReferenceException("The pointer point to null.");
                }
                PointedObject[key] = value;
            }
        }

        public override object Value
        {
            get
            {
                return PointedObject;
            }
        }

        public override string ToString()
        {
            if (this.PointedObject == null)
            {
                return "null";
            }
            return this.PointedObject.ToString();
        }

        public override Tuple<string, ResourceObject>[] GetChilds()
        {
            if (this.PointedObject == null)
            {
                return base.GetChilds();
            }
            return this.PointedObject.GetChilds();
        }
    }
}
