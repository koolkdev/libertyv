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

namespace LibertyV.Utils
{
    class ItemsRange<T>
    {
        public long Position;
        public long Size;
        public List<T> Objects;

        public ItemsRange(long position, long size)
        {
            this.Position = position;
            this.Size = size;
            this.Objects = new List<T>();
        }

        public ItemsRange(long position, long size, T obj)
            : this(position, size)
        {
            this.Objects.Add(obj);
        }

        public ItemsRange(long position, long size, List<T> objs)
        {
            this.Position = position;
            this.Size = size;
            this.Objects = objs;
        }
    }
    class Range<T>
    {

        // Key - position
        // Tuple int - size
        // Tuple T - object
        private SortedList<long, ItemsRange<T>> RangesList = new SortedList<long, ItemsRange<T>>();
        private long Size;

        public Range(long size = Int64.MaxValue)
        {
            if (size < 0) {
                throw new Exception("Invalid size");
            }
            this.Size = size;
        }

        public bool AddItem(long position, long size, T obj, bool addIfExists = true)
        {
            if (position < 0 || position > this.Size || size < 0 || position + size > this.Size)
            {
                // Invalid arguments...
                return false;
            }
            int newPos = RangesList.Keys.ToList().BinarySearch(position);
            if (newPos >= 0)
            {
                if (size == 0)
                {
                    // Hmmm, my container can't really handle empty sizes, so I won't add it
                    return true;
                }
                // Item existing, check if it has the same info
                if (addIfExists && RangesList[position].Size == size)
                {
                    RangesList[position].Objects.Add(obj);
                    return true;
                }
                return false;
            }
            newPos = ~newPos;
            long lastEnd = 0;
            if (newPos > 0)
            {
                var elem = RangesList.ElementAt(newPos - 1);
                lastEnd = elem.Key + elem.Value.Size;
            }
            if (position < lastEnd)
            {
                return false;
            }
            if (size == 0)
            {
                // Hmmm, my container can't really handle empty sizes, so I won't add it
                return true;
            }
            RangesList[position] = new ItemsRange<T>(position, size, obj);
            return true;
        }
    }
}
