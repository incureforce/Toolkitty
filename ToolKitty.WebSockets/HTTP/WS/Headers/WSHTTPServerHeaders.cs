using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public class WSHTTPServerHeaders : HTTPHeaders
    {
        public WSHTTPServerHeaders()
        {
            AddHeader("Sec-WebSocket-Version", SecWebSocketVersion);
            AddHeader("Sec-WebSocket-Protocol", SecWebSocketProtocol);
        }

        public string Upgrade {
            get => GetString(nameof(Upgrade));
            set => SetString(nameof(Upgrade), value);
        }

        public string Connection {
            get => GetString(nameof(Connection));
            set => SetString(nameof(Connection), value);
        }

        public string SecWebSocketAccept {
            get => GetString("Sec-WebSocket-Accept");
            set => SetString("Sec-WebSocket-Accept", value);
        }

        public StringListHTTPHeader SecWebSocketVersion {
            get;
        } = new StringListHTTPHeader();

        public StringListHTTPHeader SecWebSocketProtocol {
            get;
        } = new StringListHTTPHeader();
    }
}
