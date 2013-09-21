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
using System.IO;

namespace LibertyV.Utils
{
    // The point of this class, is not to close the stream on dispose
    // TODO: check if file is closed
    class PartialStream : Stream
    {
        private Stream _stream;
        private long _originalPosition;
        private long _position;
        private long _length;

        public PartialStream(Stream stream, long position, long length)
        {
            _stream = stream;
            _originalPosition = position;
            _length = length;
            if (!(_stream.CanRead)) throw new ArgumentException("Stream not readable", "stream");
            if (!(_stream.CanSeek)) throw new ArgumentException("Stream not seekable", "stream");
            if (_originalPosition < 0)
                throw new ArgumentOutOfRangeException("position", "Need non-negetive number");
            if (_length < 0)
                throw new ArgumentOutOfRangeException("length", "Need non-negetive number");
            if (_originalPosition + _length > _stream.Length)
                throw new ArgumentException("Out of original stream bounds");
            _position = 0;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _position; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Need non-negetive number");

                if (value > _length)
                    throw new ArgumentOutOfRangeException("value", "Stream length");
                _position = value;
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > _length)
                throw new ArgumentOutOfRangeException("offset", "Stream length");
            long tempPosition = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    {
                        tempPosition = offset;
                        break;
                    }
                case SeekOrigin.Current:
                    {
                        tempPosition = _position + offset;
                        break;
                    }
                case SeekOrigin.End:
                    {
                        tempPosition = _length + offset;
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid seek origin");
            }

            if (tempPosition < 0)
                throw new IOException("Seek before begin");
            if (tempPosition > _length)
                throw new IOException("Seek after end");
            _position = tempPosition;

            return _position;
        }

        public override void SetLength(long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value", "Need non-negitive number");
            if (_originalPosition + value > _stream.Length)
                throw new ArgumentException("Out of original stream bounds");
            _length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if ((long)count > _length - _position)
            {
                count = (int)(_length - _position);
            }
            // My try in making it thread safe
            lock (_stream)
            {
                _stream.Seek(_position + _originalPosition, SeekOrigin.Begin);
                _stream.Read(buffer, offset, count);
            }
            _position += count;
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Unwriteable stream");
        }

        protected override void Dispose(bool disposing)
        {
            // Don't do anything, we don't want to close the stream
            _stream = null;
        }
    }
}
