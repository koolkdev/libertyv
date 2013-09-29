using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace LibertyV.Rage.Audio.Codecs.XMA
{
    class XMA2DecoderStream : Stream
    {
        private Stream _stream;
        private IntPtr _ctx;

        [DllImport(@"xmadec.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr xma2_dec_init(int sample_rate, int channels, int bits);

        [DllImport(@"xmadec.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xma2_dec_prepare_packet(IntPtr ctx, byte[] input, int input_size);

        [DllImport(@"xmadec.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int xma2_dec_read(IntPtr ctx, byte[] output, int output_offset, int output_size);

        [DllImport(@"xmadec.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void xma2_dec_destroy(IntPtr State);

        public XMA2DecoderStream(Stream stream)
        {
            _stream = stream;
            if (!(_stream.CanRead)) throw new ArgumentException("Stream not readable", "stream");
            // need seeking for eof checking
            if (!(_stream.CanSeek)) throw new ArgumentException("Stream not seekable", "stream");

            _ctx = xma2_dec_init(32000, 1, GlobalOptions.AudioBits);
            if (_ctx == IntPtr.Zero)
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

            int originalOffset = offset;

            while (count > 0)
            {
                int read = xma2_dec_read(_ctx, buffer, offset, count);

                if (read < 0)
                {
                    throw new Exception("xma2_dec_read failed");
                }
                offset += read;
                count -= read;

                if (read == 0)
                {
                    // Read one packet
                    byte[] packet = new byte[0x800];

                    if (_stream.Read(packet, 0, packet.Length) != packet.Length) {
                        // EOF, failed to read whole packet
                        break;
                    }

                    if (xma2_dec_prepare_packet(_ctx, packet, packet.Length) < 0)
                    {
                        throw new Exception("xma2_dec_prepare_packet failed");
                    }
                }
            }

            return offset - originalOffset;
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
            if (_ctx != IntPtr.Zero)
            {
                xma2_dec_destroy(_ctx);
                _ctx = IntPtr.Zero;
            }
            _stream = null;
        }
    }
}
