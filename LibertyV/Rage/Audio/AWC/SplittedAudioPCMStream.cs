using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class SplittedAudioPCMStream : Stream
    {
        private List<Stream> _streams;
        int _currentStream = 0;

        public SplittedAudioPCMStream(List<Stream> streams)
        {
            _streams = streams;
            foreach (Stream stream in _streams)
                if (!(stream.CanRead)) throw new ArgumentException("Stream not readable", "streams");
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
