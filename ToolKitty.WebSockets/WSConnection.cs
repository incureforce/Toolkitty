using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ToolKitty.Diagnostics;

namespace ToolKitty.WebSockets
{
    public abstract class WSConnection
    {
        public event EventHandler<WSConnectionStateChangeEventArgs> StateChange;

        private WSHandler 
            handler;

        public WSConnection()
        {
        }

        public WSConnection(WSBuffers buffers)
        {
            Buffers = buffers;
        }

        public WSBuffers Buffers {
            get;
        } = WSBuffers.CreateFromDefault();

        public bool IsConnected {
            get => handler != null;
        }

        public WSHandler Handler {
            get => handler;
        }

        protected virtual void OnStateChange(WSConnectionStateChangeEventArgs eventArgs)
        {
            StateChange?.Invoke(this, eventArgs);
        }
        
        protected virtual Task<WSHandler> CreateHandler(IWSConnectionParameters parameters)
        {
            return Task.FromResult(new WSHandler(this, parameters));
        }

        protected async Task ProcessAsync(IWSConnectionParameters parameters, CancellationToken cancellationToken = default)
        {
            try {
                handler = await CreateHandler(parameters);

                if (parameters.Heartbeat > TimeSpan.Zero) {
                    _ = handler.HeartbeatAsync(cancellationToken);
                }

                OnConnect();

                while (cancellationToken.IsCancellationRequested == false) {
                    var message = await handler.ProcessAsync(cancellationToken);
                    if (message is WSCloseMessage closeMessage) {
                        return;
                    }
                }
            }
            catch (IOException ex) {
                Logging.Exception("failed to Process", ex);
                return;
            }
            finally {
                OnDisconnect();

                handler.Dispose();
                handler = null;
            }
        }

        protected virtual void OnDisconnect()
        {
            OnStateChange(new WSConnectionStateChangeEventArgs(false));
        }

        protected virtual void OnConnect()
        {
            OnStateChange(new WSConnectionStateChangeEventArgs(true));
        }
    }
}
