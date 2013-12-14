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
using System.Globalization;

namespace LibertyV.Rage.Resources.Types
{
    abstract class NumberPrimitive<T> : PrimitiveObject where T : IConvertible
    {
        protected T value;

        protected class NumberPrimitiveInfo<E> : PrimitiveTypeInfo where E : NumberPrimitive<T>
        {
            private Func<ResourceReader,T> Reader;

            public NumberPrimitiveInfo(string name, Func<ResourceReader, T> reader)
                : base(name)
            {
                this.Reader = reader;
            }

            public override ResourceObject Create()
            {
                return (E)Activator.CreateInstance(typeof(E));
            }

            public override ResourceObject Create(ResourceReader reader)
            {
                return (E)Activator.CreateInstance(typeof(E), new object[] { Reader(reader) });
            }
        }

        public NumberPrimitive(T value, TypeInfo type)
        {
            this.value = value;
            this.Type = type;
        }

        public override object Value
        {
            get
            {
                return value;
            }
        }

        public override int IntegerValue
        {
            get
            {
                return value.ToInt32(new CultureInfo("en-US"));
            }
        }

        public override uint DwordValue
        {
            get
            {
                return (uint)value.ToInt32(new CultureInfo("en-US"));
            }
        }

        public override int ShortValue
        {
            get
            {
                return value.ToInt16(new CultureInfo("en-US"));
            }
        }

        public override ushort WordValue
        {
            get
            {
                return (ushort)value.ToInt16(new CultureInfo("en-US"));
            }
        }

        public override char CharValue
        {
            get
            {
                return value.ToChar(new CultureInfo("en-US"));
            }
        }

        public override int ByteValue
        {
            get
            {
                return value.ToByte(new CultureInfo("en-US"));
            }
        }

        public override float FloatValue
        {
            get
            {
                return value.ToSingle(new CultureInfo("en-US"));
            }
        }

        public override double DoubleValue
        {
            get
            {
                return value.ToDouble(new CultureInfo("en-US"));
            }
        }
    }
}
