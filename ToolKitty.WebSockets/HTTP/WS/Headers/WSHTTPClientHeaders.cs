using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public class WSHTTPClientHeaders : HTTPHeaders
    {
        public WSHTTPClientHeaders()
        {
            AddHeader("Sec-WebSocket-Version", SecWebSocketVersion);
            AddHeader("Sec-WebSocket-Protocol", SecWebSocketProtocol);
        }

        public string Host {
            get => GetString(nameof(Host));
            set => SetString(nameof(Host), value);
        }

        public string Upgrade {
            get => GetString(nameof(Upgrade));
            set => SetString(nameof(Upgrade), value);
        }

        public string Connection {
            get => GetString(nameof(Connection));
            set => SetString(nameof(Connection), value);
        }

        public string SecWebSocketKey {
            get => GetString("Sec-WebSocket-Key");
            set => SetString("Sec-WebSocket-Key", value);
        }

        public StringListHTTPHeader SecWebSocketVersion {
            get;
        } = new StringListHTTPHeader();

        public StringListHTTPHeader SecWebSocketProtocol {
            get;
        } = new StringListHTTPHeader();
    }
}
