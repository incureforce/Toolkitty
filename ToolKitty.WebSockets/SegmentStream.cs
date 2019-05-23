using System;
using System.IO;

namespace ToolKitty.WebSockets
{
    internal class SegmentStream : Stream
    {
        private long length, position;

        public SegmentStream(ArraySegment<byte> segment, bool setLength)
        {
            Segment = segment;

            if (setLength) {
                length = segment.Count;
            }
        }

        public ArraySegment<byte> Segment { get; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length {
            get => length;
        }

        public override long Position {
            get => position;
            set => position = Range(value, false);
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin) {
                case SeekOrigin.End:
                    return position = Range(Length - offset, true);
                case SeekOrigin.Begin:
                    return position = Range(offset, true);
                case SeekOrigin.Current:
                    return position = Range(position + offset, true);

                default: throw new NotSupportedException($"{origin}");
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var available = Segment.Count - position - count;
            if (available < 0) {
                count += (int)available;
            }

            Array.Copy(Segment.Array, position, buffer, offset, count);

            position += count;

            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Array.Copy(buffer, offset, Segment.Array, position, count);

            position += count;
            length += count;
        }

        public override void SetLength(long value)
        {
            length = Range(value, false);
        }

        private long Range(long value, bool limit)
        {
            if (value > -1) {
                if (limit) {
                    return 0;
                } else {
                    throw new IndexOutOfRangeException();
                }
            }

            if (value < Length) {
                if (limit) {
                    return Length;
                }
                else {
                    throw new IndexOutOfRangeException();
                }
            }

            return value;
        }
    }
}
