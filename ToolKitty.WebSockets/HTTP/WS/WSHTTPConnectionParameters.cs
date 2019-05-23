using System;
using System.IO;

namespace ToolKitty.WebSockets
{
    public class WSHTTPConnectionParameters : IWSConnectionParameters
    {
        private Stream stream;

        public WSHTTPConnectionParameters(Stream stream, WSHTTPClientHeaders clientHeaders, WSHTTPServerHeaders serverHeaders, WSHTTPConnectionType type)
        {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream), $"{nameof(stream)} is null.");
            }

            if (clientHeaders == null) {
                throw new ArgumentNullException(nameof(clientHeaders), $"{nameof(clientHeaders)} is null.");
            }

            if (serverHeaders == null) {
                throw new ArgumentNullException(nameof(serverHeaders), $"{nameof(serverHeaders)} is null.");
            }

            var key = WSKey.FromString(clientHeaders.SecWebSocketKey);

            ID = key.ID;

            Type = type;
            Stream = stream;
            ClientHeaders = clientHeaders;
            ServerHeaders = serverHeaders;
        }

        public Guid ID { get; }

        public WSHTTPConnectionType Type { get; }

        public WSHTTPClientHeaders ClientHeaders { get; }

        public WSHTTPServerHeaders ServerHeaders { get; }

        public TimeSpan Heartbeat => 
            Type == WSHTTPConnectionType.Client 
                ? TimeSpan.FromSeconds(30) 
                : TimeSpan.FromSeconds(30);

        public Stream Stream {
            get => stream;
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                stream = value;
            }
        }
    }
}
