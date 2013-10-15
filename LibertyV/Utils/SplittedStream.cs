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

namespace LibertyV.Rage.Audio.AWC
{
    class SplittedStream : Stream
    {
        private List<Stream> _streams;
        int _currentStream = 0;
        long _position = 0;

        public SplittedStream(List<Stream> streams)
        {
            _streams = streams;
            foreach (Stream stream in _streams)
            {
                if (!(stream.CanRead)) throw new ArgumentException("Stream not readable", "streams");
                if (!(stream.CanSeek)) throw new ArgumentException("Stream not seekable", "streams");
            }
        }

        private void UpdatePosition(long pos)
        {
            if (pos == _position)
            {
                return;
            }
            long tempPos = pos;
            int newStream = 0;
            while (tempPos > _streams[newStream].Length)
            {
                _streams[newStream].Seek(tempPos, SeekOrigin.End);
                tempPos -= _streams[newStream].Length;
                newStream += 1;
                if (newStream == _streams.Count)
                {
                    throw new Exception("Position is out of bounds");
                }
            }
            _streams[newStream].Seek(tempPos, SeekOrigin.Begin);
            _currentStream = newStream;
            _position = pos;
            for (++newStream; newStream < _streams.Count; ++newStream)
            {
                _streams[newStream].Seek(0, SeekOrigin.Begin);
            }
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
            get { return _streams.Sum(stream => stream.Length); }
        }

        public override long Position
        {
            get { return _position; }
            set { UpdatePosition(value); }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
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
                        tempPosition = Length + offset;
                        break;
                    }
                default:
                    throw new ArgumentException("Invalid seek origin");
            }

            if (tempPosition != _position)
            {
                if (tempPosition < 0)
                    throw new IOException("Seek before begin");
                if (tempPosition > Length)
                    throw new IOException("Seek after end");
                UpdatePosition(tempPosition);
            }

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Can't change stream size");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Need non-negitive number");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need non-negitive number");
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset and length");

            int startOffset = offset;
            while (count > 0)
            {
                if (_currentStream == _streams.Count)
                {
                    // EOF
                    break;
                }
                int read = _streams[_currentStream].Read(buffer, offset, count);

                if (read < count)
                {
                    // eof, next stream
                    _currentStream += 1;
                }

                _position += read;
                count -= read;
                offset += read;
            }

            return offset - startOffset;
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Unwriteable stream");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_streams != null)
                {
                    foreach (Stream stream in _streams)
                    {
                        stream.Close();
                    }
                    _streams = null;
                }
            }
        }
    }
}
