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
        delegate int IsEOFDelegate();

        ReadDelegate readFunc;
        IsEOFDelegate isEOFFunc;

        [DllImport(@"libmad.dll")]
        private static extern IntPtr mpeg_decoder_init(int bits, ReadDelegate readFunc, IsEOFDelegate isEOF);

        [DllImport(@"libmad.dll")]
        private static extern int mpeg_decoder_process(IntPtr State, byte[] pDestination, int Offset, int DestSize);

        [DllImport(@"libmad.dll")]
        private static extern int mpeg_decoder_destroy(IntPtr State);

        private int IsEOF()
        {
            return (_stream.Position == _stream.Length) ? 1 : 0;
        }

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
            isEOFFunc = IsEOF;
            _state = mpeg_decoder_init(GlobalOptions.AudioBits, readFunc, isEOFFunc);
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

            int read = mpeg_decoder_process(_state, buffer, offset, count);

            if (read < 0)
            {
                throw new Exception("LZXdecompress failed");
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
                mpeg_decoder_destroy(_state);
                _state = IntPtr.Zero;
            }
            _stream = null;
            readFunc = null;
            isEOFFunc = null;
        }
    }
}
