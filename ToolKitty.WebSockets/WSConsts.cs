namespace ToolKitty.WebSockets
{
    //public class WSStream : Stream
    //{
    //    private long length, position;

    //    public WSStream(WSStreamOperations wSStreamOperations)
    //    {
    //        if (operations == null) {
    //            throw new ArgumentNullException(nameof(operations));
    //        }

    //        this.length = length;

    //        Operations = operations;
    //    }

    //    public override bool CanRead => true;

    //    public override bool CanSeek => true;

    //    public override bool CanWrite => false;

    //    public override long Length {
    //        get => length;
    //    }

    //    public WSStreamOperations Operations {
    //        get;
    //    }

    //    public override long Position {
    //        get => position;
    //        set => throw new NotImplementedException();
    //    }

    //    public override void Flush()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override int Read(byte[] buffer, int offset, int count)
    //    {
    //        var segment = new ArraySegment<byte>(buffer, offset, count);
    //        var segmentOffset = Operations.ReadPayload(segment);

    //        position += segmentOffset;

    //        return segmentOffset;
    //    }

    //    public override long Seek(long offset, SeekOrigin origin)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void SetLength(long value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override void Write(byte[] buffer, int offset, int count)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public static class WSConsts
    {
        public const int
            UInt00Limit = 0x7D,
            UInt16Limit = 0xFFFF;
        public const byte
            MSB = 0x80,
            RSV0 = 0x40,
            RSV1 = 0x20,
            RSV2 = 0x10,
            CodeMask = 0x0F,
            LengthMask = 0x7F,

            UInt16Flag = 0x7E,
            UInt64Flag = 0x7F,

            CodeContinue = 0x0,
            CodeContentText = 0x1,
            CodeContentBinary = 0x2,
            CodeClose = 0x8,
            CodePing = 0x9,
            CodePong = 0xA;

        public const string 
            KeySuffix = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
    }
}
