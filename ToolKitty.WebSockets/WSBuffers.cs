using System;
using System.Buffers;

namespace ToolKitty.WebSockets
{
    public struct WSBuffers
    {
        public StrideBuffer<byte> ByteBuffer;
        public StrideBuffer<char> CharBuffer;

        public static readonly StrideBuffer<byte>
            DefaultByteBuffer = new StrideBuffer<byte>();
        public static readonly StrideBuffer<char>
            DefaultCharBuffer = new StrideBuffer<char>();

        public bool IsIncomplete {
            get => ByteBuffer == null
                || CharBuffer == null;
        }

        public static WSBuffers CreateFromDefault()
        {
            return new WSBuffers {
                ByteBuffer = DefaultByteBuffer,
                CharBuffer = DefaultCharBuffer,
            };
        }
    }
}
