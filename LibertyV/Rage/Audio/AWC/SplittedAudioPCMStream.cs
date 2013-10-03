using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class SplittedAudioPCMStream : Stream
    {

        private class RawStream : Stream
        {
            private List<Stream> _streams;
            int _currentStream = 0;

            public void NextStream()
            {
                _currentStream += 1;
                _currentStream %= _streams.Count;
            }

            public RawStream(List<Stream> streams)
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
                return _streams[_currentStream].Read(buffer, offset, count);
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
        
        private RawStream _rawStream;
        private Stream _decStream = null;
        int _currentStream = 0;
        int[] _streamSkipBytes;
        int[] _streamsBytes;

        public SplittedAudioPCMStream(List<Stream> streams, List<Tuple<int, int>> streamsSamples, int channels, int bits)
        {
            _rawStream = new RawStream(streams);
            _streamSkipBytes = streamsSamples.Select(samples => (int)(samples.Item1 * (bits / 8) * channels)).ToArray();
            _streamsBytes = streamsSamples.Select(samples => (int)(samples.Item2 * (bits / 8) * channels)).ToArray();
        }

        public Stream GetRawReadStream()
        {
            return _rawStream;
        }

        public void SetDecoderStream(Stream stream)
        {
            _decStream = stream;
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
            if (_decStream == null)
            {
                throw new Exception("Decoder stream is null");
            }
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Need non-negitive number");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Need non-negitive number");
            if (buffer.Length - offset < count)
                throw new ArgumentException("Invalid offset and length");

            int startOffset = offset;
            while (count > 0)
            {
                if (_currentStream == _streamsBytes.Length)
                {
                    // EOF
                    break;
                }
                int toRead;
                if (_streamSkipBytes[_currentStream] != 0)
                {
                    byte[] skipBytes = new byte[_streamSkipBytes[_currentStream] < 0x8000 ? _streamSkipBytes[_currentStream] : 0x8000];
                    while (_streamSkipBytes[_currentStream] > 0)
                    {
                        toRead = _streamSkipBytes[_currentStream] < skipBytes.Length ? _streamSkipBytes[_currentStream] : skipBytes.Length;
                        if (_decStream.Read(skipBytes, 0, toRead) != toRead)
                        {
                            throw new Exception("Bad stream, read unexcepted amount of samples");
                        }
                        _streamSkipBytes[_currentStream] -= toRead;
                    }
                }
                toRead = (count < _streamsBytes[_currentStream]) ? count : _streamsBytes[_currentStream];
                if (_decStream.Read(buffer, offset, toRead) != toRead)
                {
                    throw new Exception("Bad stream, read unexcepted amount of samples");
                }
                _streamsBytes[_currentStream] -= toRead;
                count -= toRead;
                offset += toRead;
                if (_streamsBytes[_currentStream] == 0)
                {
                    // empty the current stream
                    byte[] skipBytes = new byte[0x8000];
                    while (_decStream.Read(skipBytes, 0, skipBytes.Length) != 0) ;
                    ++_currentStream;
                    _rawStream.NextStream();
                }
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
                if (_decStream != null)
                {
                    _decStream.Close();
                    _decStream = null;
                }
                if (_rawStream != null)
                {
                    _rawStream.Close();
                    _rawStream = null;
                }
            }
            _streamsBytes = null;
        }
    }
}