using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolKitty.Diagnostics;

namespace ToolKitty.WebSockets
{
    public class WSOperations
    {
        static readonly ILogger
            Logger = ContextLogger.FromCallStack();

        private static readonly RandomNumberGenerator
            RNG = new RNGCryptoServiceProvider();
        private static readonly SHA1
            SHA1 = SHA1.Create();
        private static readonly Encoding
            UTF8 = Encoding.UTF8;

        private readonly byte[]
            frame = { 0, 0 },
            frameMask = { 0, 0, 0, 0 },
            frameLength = { 0, 0, 0, 0, 0, 0, 0, 0 };

        private int maskOffset;
        private readonly SemaphoreSlim
            semaphore = new SemaphoreSlim(1, 1);

        public WSOperations(WSBuffers buffers, IWSConnectionParameters options)
        {
            if (options == null) {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} is null.");
            }

            if (buffers.IsIncomplete) {
                throw new ArgumentException($"{nameof(buffers)} is incomplete", nameof(buffers));
            }

            Stream = options.Stream;
            Buffer = buffers.ByteBuffer;
        }

        protected Stream Stream {
            get;
        }

        protected StrideBuffer<byte> Buffer {
            get;
        }

        protected void ResetMask()
        {
            maskOffset = 0;

            Array.Clear(frameMask, 0, 4);
        }

        protected void CreateMask()
        {
            Scramble(frameMask, 0, 4);
        }

        public static void Scramble(ArraySegment<byte> segment)
        {
            RNG.GetBytes(segment.Array, segment.Offset, segment.Count);
        }

        public static string Checksum(WSKey key)
        {
            var bytes = UTF8.GetBytes(key.Text + WSConsts.KeySuffix);

            return Convert.ToBase64String(bytes);
        }

        public static void Scramble(byte[] buffer, int offset, int length)
        {
            RNG.GetBytes(buffer, offset, length);
        }

        protected void ApplyMask(ArraySegment<byte> buffer)
        {
            for (var i = 0; i < buffer.Count; ++i) {
                buffer.Array[i] ^= frameMask[maskOffset++ % 4];
            }
        }

        public async Task<WSFrame> ReadFrame(CancellationToken cancellationToken)
        {
            try {
                await semaphore.WaitAsync();

                Logger.Debug($"ReadFrame");

                var frameHeader = await ReadFrameHeader(cancellationToken);

                Logger.Debug($"ReadFrame (header complete)", new {
                    Header = frameHeader,
                });

                var fragment = Buffer.GetFragment(frameHeader.Length);

                ReadPayload(fragment.GetSegment());

                return new WSFrame {
                    Last = frameHeader.Last,
                    Code = frameHeader.Code,
                    Fragment = fragment,
                };
            }
            finally {
                semaphore.Release();
            }
        }

        public async Task<WSFrame> SendFrame(WSFrame frame, CancellationToken cancellationToken)
        {
            try {
                await semaphore.WaitAsync();

                var fragment = frame.Fragment;

                var frameHeader = new WSFrameHeader {
                    Mask = true,
                    Last = frame.Last,
                    Code = frame.Code,
                    Length = fragment.Length,
                };

                Logger.Debug($"SendFrame");

                await SendFrameHeader(frameHeader, cancellationToken);

                Logger.Debug($"SendFrame (header complete)", new {
                    Header = frameHeader,
                });

                SendPayload(fragment.GetSegment());

                return frame;
            }
            finally {
                semaphore.Release();
            }
        }

        private async Task<WSFrameHeader> SendFrameHeader(WSFrameHeader frameHeader, CancellationToken cancellationToken)
        {
            Array.Clear(frame, 0, 2);
            Array.Clear(frameMask, 0, 4);
            Array.Clear(frameLength, 0, 8);

            var frameHeaderLength = GetLength(frameHeader.Length);

            var frame0 = 0;

            if (frameHeader.Last) {
                frame0 |= WSConsts.MSB;
            }

            if (frameHeader.RSV0) {
                frame0 |= WSConsts.RSV0;
            }

            if (frameHeader.RSV1) {
                frame0 |= WSConsts.RSV1;
            }

            if (frameHeader.RSV2) {
                frame0 |= WSConsts.RSV2;
            }

            frame0 |= frameHeader.Code & WSConsts.CodeMask;

            var frame1 = 0;

            if (frameHeader.Mask) {
                frame1 |= WSConsts.MSB;
            }

            frame1 |= frameHeaderLength & WSConsts.LengthMask;

            frame[0] = (byte)frame0;
            frame[1] = (byte)frame1;

            await Stream.WriteAsync(frame, 0, 2, cancellationToken);

            ResetMask();

            if (frameHeader.Mask) {
                CreateMask();

                await Stream.WriteAsync(frameMask, 0, 4, cancellationToken);
            }

            EncodeLength(frameHeader.Length);

            switch (frameHeaderLength) {
                case WSConsts.UInt16Flag:
                    await Stream.WriteAsync(frameLength, 6, 2, cancellationToken);
                    break;
                case WSConsts.UInt64Flag:
                    await Stream.WriteAsync(frameLength, 0, 8, cancellationToken);
                    break;
            }

            return frameHeader;
        }

        private int GetLength(int length)
        {
            if (length > WSConsts.UInt16Limit) {
                return WSConsts.UInt64Flag;
            }

            if (length > WSConsts.UInt00Limit) {
                return WSConsts.UInt16Flag;
            }

            return length;
        }

        private async Task<WSFrameHeader> ReadFrameHeader(CancellationToken cancellationToken)
        {
            Array.Clear(frame, 0, 2);
            Array.Clear(frameMask, 0, 4);
            Array.Clear(frameLength, 0, 8);

            await Stream.ReadAsync(frame, 0, 2);

            var last = (frame[0] & WSConsts.MSB) != 0;
            var mask = (frame[1] & WSConsts.MSB) != 0;
            var rsv0 = (frame[0] & WSConsts.RSV0) != 0;
            var rsv1 = (frame[0] & WSConsts.RSV1) != 0;
            var rsv2 = (frame[0] & WSConsts.RSV2) != 0;

            var code = frame[0] & WSConsts.CodeMask;
            var length = frame[1] & WSConsts.LengthMask;

            ResetMask();

            if (mask) {
                await Stream.ReadAsync(frameMask, 0, 4, cancellationToken);
            }

            switch (length) {
                case WSConsts.UInt16Flag:
                    await Stream.ReadAsync(frameLength, 6, 2, cancellationToken);

                    length = 0;

                    DecodeLength(ref length);
                    break;
                case WSConsts.UInt64Flag:
                    await Stream.ReadAsync(frameLength, 0, 8, cancellationToken);

                    length = 0;

                    DecodeLength(ref length);
                    break;
            }

            var header = new WSFrameHeader {
                Last = last,
                Mask = mask,
                Code = code,
                RSV0 = rsv0,
                RSV1 = rsv1,
                RSV2 = rsv2,
                Length = length,
            };

            return header;
        }

        // optimized decode length
        private void DecodeLength(ref int length)
        {
            if ((frameLength[0] | frameLength[1] | frameLength[2] | frameLength[3] | (frameLength[4] & WSConsts.MSB)) != 0) { // MSB indicates int overflow and all other bytes also exceed integer 32 bit range
                throw new OutOfMemoryException();
            }

            // length |= (frameLength[0] << 56);
            // length |= (frameLength[1] << 48);
            // length |= (frameLength[2] << 40);
            // length |= (frameLength[3] << 32);
            length |= (frameLength[4] << 24);
            length |= (frameLength[5] << 16);
            length |= (frameLength[6] << 08);
            length |= (frameLength[7] << 00);
        }

        private void EncodeLength(int length)
        {
            // 0 - 3 exceed integer 32 value
            frameLength[0] = 0;
            frameLength[1] = 0;
            frameLength[2] = 0;
            frameLength[3] = 0;
            frameLength[4] = (byte)(0xFF & (length >> 24));
            frameLength[5] = (byte)(0xFF & (length >> 16));
            frameLength[6] = (byte)(0xFF & (length >> 08));
            frameLength[7] = (byte)(0xFF & (length >> 00));
        }

        internal int ReadPayload(ArraySegment<byte> segment)
        {
            const int chunkLimit = 1024;

            var segmentOffset = 0;
            var segmentCount = segment.Count;

            using (var fragment = Buffer.GetFragment(chunkLimit)) {
                var buffer = fragment.GetSegment();

                do {
                    var chunkCount = Math.Min(segmentCount - segmentOffset, chunkLimit);
                    var chunk = fragment.GetSegment(0, chunkCount);

                    var bufferCount = Stream.Read(chunk.Array, chunk.Offset, chunkCount);
                    if (bufferCount != chunkCount) {
                        throw new NotSupportedException("Missing Data");
                    }

                    ApplyMask(chunk);

                    Array.Copy(chunk.Array, chunk.Offset, segment.Array, segment.Offset + segmentOffset, chunk.Count);

                    segmentOffset += chunkCount;
                } while (segmentCount < segmentOffset);

                return segmentCount;
            }
        }

        private void SendPayload(ArraySegment<byte> segment)
        {
            const int chunkLimit = 1024;

            var segmentOffset = 0;
            var segmentCount = segment.Count;

            using (var fragment = Buffer.GetFragment(chunkLimit)) {
                var buffer = fragment.GetSegment();

                do {
                    var chunkCount = Math.Min(segmentCount - segmentOffset, chunkLimit);
                    var chunk = fragment.GetSegment(0, chunkCount);

                    Array.Copy(segment.Array, segment.Offset + segmentOffset, chunk.Array, chunk.Offset, chunk.Count);

                    ApplyMask(chunk);

                    Stream.Write(chunk.Array, chunk.Offset, chunkCount);

                    segmentOffset += chunkCount;
                } while (segmentOffset < segmentCount);
            }
        }
    }
}
