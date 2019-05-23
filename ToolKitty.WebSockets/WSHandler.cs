using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToolKitty.Diagnostics;

namespace ToolKitty.WebSockets
{
    public class WSHandler : IDisposable
    {
        static readonly Encoding UTF8 = Encoding.UTF8;

        public WSHandler(WSConnection connection, IWSConnectionParameters options)
        {
            if (connection == null) {
                throw new ArgumentNullException(nameof(connection), $"{nameof(connection)} is null.");
            }

            if (options == null) {
                throw new ArgumentNullException(nameof(options), $"{nameof(options)} is null.");
            }


            var buffers = connection.Buffers;

            if (buffers.IsIncomplete) {
                throw new NotSupportedException($"{nameof(WSConnection.Buffers)} is incomplete");
            }

            Connection = connection;
            ReadOperations = new WSOperations(buffers, options);
            SendOperations = new WSOperations(buffers, options);

            Stopwatch = new Stopwatch();

            Heartbeat = options.Heartbeat;
            ByteBuffer = buffers.ByteBuffer;
            CharBuffer = buffers.CharBuffer;
        }


        private readonly TimeSpan Heartbeat;
        private readonly Stopwatch Stopwatch;

        private readonly Pulser<WSFrame>
            Pulser = new Pulser<WSFrame>();
        private readonly TaskCompletionSource<bool>
            HeartbeatTaskCompletionSource = new TaskCompletionSource<bool>();

        private readonly SemaphoreSlim
            Semaphore = new SemaphoreSlim(1, 1);

        public WSConnection Connection { get; }

        protected StrideBuffer<byte> ByteBuffer { get; }
        protected StrideBuffer<char> CharBuffer { get; }

        protected WSOperations ReadOperations { get; }
        protected WSOperations SendOperations { get; }

        public async Task SendFrame(WSFrame frame, CancellationToken cancellationToken = default)
        {
            using (await WaitForSendLock()) {
                await SendOperations.SendFrame(frame, cancellationToken);
            }
        }

        public async Task HeartbeatAsync(CancellationToken cancellationToken = default)
        {
            if (Heartbeat < TimeSpan.FromMilliseconds(1)) {
                return;
            }

            if (Stopwatch.IsRunning) {
                return;
            }

            Stopwatch.Restart();

            var task = HeartbeatTaskCompletionSource.Task;
            var random = new Random();
            var pulser = default(Task<bool>);
            var readFrame = default(WSFrame);

            using (var sendFragment = ByteBuffer.GetFragment(200)) {
                var sendSegment = sendFragment.GetSegment();

                while (cancellationToken.IsCancellationRequested == false) {
                    var delay = Heartbeat - Stopwatch.Elapsed;
                    if (delay.Ticks > 0) {
                        var result = await Task.WhenAny(task, Task.Delay(delay));

                        if (result == task) {
                            break;
                        }
                    }
                    else {
                        using (await WaitForSendLock(cancellationToken)) {
                            WSOperations.Scramble(sendSegment);

                            pulser = Task.Run(delegate {
                                return Pulser.Wait(out readFrame, 3000);
                            });

                            await SendOperations.SendFrame(new WSFrame {
                                Fragment = sendFragment,
                                Code = WSConsts.CodePing,
                                Last = true,
                            }, cancellationToken);
                        }

                        if (await pulser) {
                            using (var readFragment = readFrame.Fragment) {
                                var readSegment = readFragment.GetSegment();

                                if (readFrame.Code == WSConsts.CodePong && readFrame.Last && readSegment.SequenceEqual(sendSegment)) {
                                    continue;
                                }
                                else {
                                    throw new NotSupportedException("Pong is incorrect");
                                }
                            }
                        }

                        throw new NotSupportedException("Pong is incorrect");
                    }
                }

                Stopwatch.Reset();
            }
        }

        public virtual async Task<IWSMessage> ProcessAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            while (cancellationToken.IsCancellationRequested == false) {
                var frame = await ReadOperations.ReadFrame(cancellationToken);

                if (Stopwatch.IsRunning) {
                    Stopwatch.Restart();
                }

                switch (frame.Code) {
                    case WSConsts.CodePong:
                        Pulser.Push(frame, false);
                        continue;
                    case WSConsts.CodePing:
                        await SendPingBack(frame, cancellationToken);
                        continue;

                    case WSConsts.CodeClose:
                    case WSConsts.CodeContentText:
                    case WSConsts.CodeContentBinary:
                        return await ToMessage(frame, cancellationToken);

                }
            }

            return default;
        }

        public virtual async Task SendAsync(IWSMessage message, CancellationToken cancellationToken = default)
        {
            switch (message.Code) {
                case WSConsts.CodeContentText:
                    await SendTextAsync((WSTextMessage)message, cancellationToken);
                    return;
                case WSConsts.CodeClose:
                    await SendCloseAsync((WSCloseMessage)message, cancellationToken);
                    return;
            }
        }

        public async Task SendTextAsync(WSTextMessage message, CancellationToken cancellationToken = default)
        {
            var text = message.Text;
            var bytesLength = UTF8.GetByteCount(text);

            using (var byteFragment = ByteBuffer.GetFragment(bytesLength)) {
                var bytes = byteFragment.GetSegment();

                EncodeText(bytes, text);

                using (await WaitForSendLock()) {
                    await SendOperations.SendFrame(new WSFrame {
                        Last = true,
                        Code = WSConsts.CodeContentText,
                        Fragment = byteFragment,
                    }, cancellationToken);
                }
            }
        }

        public async Task SendCloseAsync(WSCloseMessage message, CancellationToken cancellationToken = default)
        {
            HeartbeatTaskCompletionSource.TrySetResult(true);

            var text = message.StatusText;
            var code = message.StatusCode;
            var bytesLength = UTF8.GetByteCount(text) + 2;

            using (var byteFragment = ByteBuffer.GetFragment(bytesLength)) {
                var codeBytes = byteFragment.GetSegment(0, 2);
                var textBytes = byteFragment.GetSegment(2, bytesLength - 2);

                EncodeCode(codeBytes, code);
                EncodeText(textBytes, text);

                using (await WaitForSendLock()) {
                    await SendOperations.SendFrame(new WSFrame {
                        Last = true,
                        Code = WSConsts.CodeContentText,
                        Fragment = byteFragment,
                    }, cancellationToken);
                }
            }
        }

        public void Dispose()
        {
            OnDispose();

            HeartbeatTaskCompletionSource.TrySetResult(true);
        }

        protected virtual async Task<IWSMessage> ToMessage(WSFrame frame, CancellationToken cancellationToken)
        {
            switch (frame.Code) {
                case WSConsts.CodeClose:
                    return await ToCloseMessage(frame);
                case WSConsts.CodeContentText:
                    return await ToTextMessage(frame);
                default: throw new NotSupportedException($"Message type not supported");
            }
        }

        protected Task<WSCloseMessage> ToCloseMessage(WSFrame frame)
        {
            HeartbeatTaskCompletionSource.TrySetResult(true);

            if (frame.Code != WSConsts.CodeClose) {
                throw new NotSupportedException();
            }

            if (frame.Last == false) {
                throw new NotSupportedException();
            }

            using (var byteFragment = frame.Fragment) {
                var code = default(ushort);
                var text = default(string);

                int length = byteFragment.Length;
                if (length > 2) {
                    var textBytes = byteFragment.GetSegment(2, length - 2);

                    text = DecodeText(textBytes);
                }
                else
                if (length > 0) {
                    var codeBytes = byteFragment.GetSegment(0, 2);

                    code = DecodeCode(codeBytes);
                }

                return Task.FromResult(new WSCloseMessage {
                    StatusCode = code,
                    StatusText = text,
                });
            }
        }

        protected Task<WSTextMessage> ToTextMessage(WSFrame frame)
        {
            if (frame.Code != WSConsts.CodeContentText) {
                throw new NotSupportedException();
            }

            if (frame.Last == false) {
                throw new NotSupportedException();
            }

            using (var byteFragment = frame.Fragment) {
                var bytes = byteFragment.GetSegment();

                return Task.FromResult(new WSTextMessage {
                    Text = DecodeText(bytes),
                });
            }
        }

        protected void EncodeCode(ArraySegment<byte> bytes, ushort code)
        {
            var buffer = BitConverter.GetBytes(code);

            bytes.Array[0] = buffer[0];
            bytes.Array[1] = buffer[1];
        }

        protected void EncodeText(ArraySegment<byte> bytes, string text)
        {
            using (var charFragment = CharBuffer.GetFragment(text.Length)) {
                var chars = charFragment.GetSegment();

                text.CopyTo(0, chars.Array, chars.Offset, chars.Count);

                UTF8.GetBytes(chars.Array, chars.Offset, chars.Count, bytes.Array, bytes.Offset);
            }
        }

        protected ushort DecodeCode(ArraySegment<byte> bytes)
        {
            return BitConverter.ToUInt16(bytes.Array, 0);
        }

        protected string DecodeText(ArraySegment<byte> bytes)
        {
            var charsLength = UTF8.GetCharCount(bytes.Array, bytes.Offset, bytes.Count);

            using (var charFragment = CharBuffer.GetFragment(charsLength)) {
                var chars = charFragment.GetSegment();

                UTF8.GetChars(bytes.Array, bytes.Offset, bytes.Count, chars.Array, chars.Offset);

                return new string(chars.Array, chars.Offset, chars.Count);
            }
        }

        protected async Task<IDisposable> WaitForSendLock(CancellationToken cancellationToken = default)
        {
            await Semaphore.WaitAsync(cancellationToken);

            return Disposer.Use(Disposer_Dispose);
        }

        protected virtual void OnDispose()
        {
        }

        private void Disposer_Dispose(object sender, EventArgs eventArgs)
        {
            Semaphore.Release();
        }

        private async Task SendPingBack(WSFrame frame, CancellationToken cancellationToken)
        {
            using (await WaitForSendLock(cancellationToken)) {
                do {
                    switch (frame.Code) {
                        case WSConsts.CodePing:
                            frame.Code = WSConsts.CodePong;
                            break;
                        case WSConsts.CodeContinue:
                            break;

                        default: throw new Exception("Data Exception");
                    }

                    await SendOperations.SendFrame(frame, cancellationToken);

                    if (frame.Last == false) {
                        frame = await ReadOperations.ReadFrame(cancellationToken);
                    }
                } while (cancellationToken.IsCancellationRequested == false && frame.Last == false);

                if (Stopwatch.IsRunning) {
                    Stopwatch.Restart();
                }
            }
        }
    }
}
