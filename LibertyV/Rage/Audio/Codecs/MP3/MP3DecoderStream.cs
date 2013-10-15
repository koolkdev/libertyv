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
using System.Runtime.InteropServices;

namespace LibertyV.Rage.Audio.Codecs.MP3
{
    class MP3DecoderStream : Stream
    {
        private Stream _stream;
        private IntPtr _state;

        delegate int ReadDelegate(IntPtr destPtr, int offset, int count);

        ReadDelegate readFunc;

        [DllImport(@"libav_wrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr mp3_dec_init(ReadDelegate readFunc, int bits);

        [DllImport(@"libav_wrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int mp3_dec_read(IntPtr ctx, byte[] output, int output_offset, int output_size);

        [DllImport(@"libav_wrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void mp3_dec_free(IntPtr State);


        private int UnsafeRead(IntPtr destPtr, int offset, int count)
        {
            byte[] dest = new byte[count];
            int res = _stream.Read(dest, offset, count);
            Marshal.Copy(dest, 0, destPtr, res);
            return res;
        }

        public MP3DecoderStream(Stream stream)
        {
            _stream = stream;
            if (!(_stream.CanRead)) throw new ArgumentException("Stream not readable", "stream");
            // need seeking for eof checking
            if (!(_stream.CanSeek)) throw new ArgumentException("Stream not seekable", "stream");

            readFunc = UnsafeRead;
            _state = mp3_dec_init(readFunc, GlobalOptions.AudioBits);
            if (_state == IntPtr.Zero)
            {
                throw new Exception("XMemCreateDecompressionContext failed");
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException("Unseekable Stream"); }
        }

        public override long Position
        {
            get { throw new NotSupportedException("Unseekable Stream"); }
            set { throw new NotSupportedException("Unseekable Stream"); }
        }

        public override void Flush()
        {
            this._stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Unseekable Stream");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("Unseekable Stream");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Need non-negitive number");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need non-negitive number");
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset and length");

            int read = mp3_dec_read(_state, buffer, offset, count);

            if (read < 0)
            {
                throw new Exception("mpeg_decoder_process failed");
            }

            return read;
        }


        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Unwriteable stream");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Close();
            }
            if (_state != IntPtr.Zero)
            {
                mp3_dec_free(_state);
                _state = IntPtr.Zero;
            }
            _stream = null;
            readFunc = null;
        }
    }
}
