using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibertyV.Rage.Audio.AWC
{
    class MultiChannelPCMStream : Stream
    {
        private List<Stream> _streams;
        int _currentChannel = 0;
        int _currentSample = 0;
        int _currentSampleSize = 0;
        int _sampleSize;
        int _samples;

        public MultiChannelPCMStream(List<Stream> streams, uint samples, int bits)
        {
            _streams = streams;
            foreach (Stream stream in _streams)
                if (!(stream.CanRead)) throw new ArgumentException("Stream not readable", "streams");

            _sampleSize = bits / 8;
            _samples = (int)samples;
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

            int channels = _streams.Count;
            int startOffset = offset;
            // If we are in the middle of a channel, let's write it first
            if (_currentSampleSize != 0 && count > 0)
            {
                do
                {
                    int toRead = count < _currentSampleSize ? count : _currentSampleSize;

                    if (_streams[_currentChannel].Read(buffer, offset, toRead) != toRead)
                    {
                        throw new Exception("Invalid channel stream");
                    }

                    count -= toRead;
                    offset += toRead;
                    _currentSampleSize -= toRead;

                    if (_currentSampleSize == 0)
                    {
                        _currentChannel = (_currentChannel + 1) % channels;
                        _currentSampleSize = _sampleSize;
                    }
                }
                while (_currentChannel != 0 && count > 0);
            }
            _currentSampleSize = 0;

            // Calculate how many full samples are we going to read
            int samplesToRead = count / (_sampleSize * channels);
            if (_currentSample + samplesToRead > _samples)
            {
                samplesToRead = _samples - _currentSample;
            }

            // TODO: smaller allocations?
            byte[] tempBuffer = new byte[_sampleSize * samplesToRead];
            for (int i=0; i < channels; ++i)
            {
                if (_streams[i].Read(tempBuffer, 0, tempBuffer.Length) != tempBuffer.Length)
                {
                    throw new Exception("Invalid channel stream");
                }
                int pos = offset + i * _sampleSize;
                int tempBufferPos = 0;
                for (int j = 0; j < samplesToRead; ++j)
                {
                    for (int k = 0; k < _sampleSize; ++k)
                    {
                        buffer[pos + k] = tempBuffer[tempBufferPos++];
                    }
                    pos += _sampleSize * channels;
                }
            }
            count -= _sampleSize * samplesToRead * channels;
            offset += _sampleSize * samplesToRead * channels;
            _currentSample += samplesToRead;

            // We still have uncompleted sample to write
            if (count != 0 && _currentSample < _samples)
            {
                ++_currentSample;
                while (count > 0)
                {
                    int toRead = count < _sampleSize ? count : _sampleSize;

                    if (_streams[_currentChannel].Read(buffer, offset, toRead) != toRead)
                    {
                        throw new Exception("Invalid channel stream");
                    }

                    count -= toRead;
                    offset += toRead;
                    _currentSampleSize = _sampleSize - toRead;

                    if (toRead == _sampleSize)
                    {
                        _currentChannel = (_currentChannel + 1) % _streams.Count;
                    }
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
