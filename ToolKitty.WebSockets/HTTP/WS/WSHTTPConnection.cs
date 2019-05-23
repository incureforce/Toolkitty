using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ToolKitty.WebSockets
{
    public abstract class WSHTTPConnection : WSConnection
    {
        public WSHTTPConnection()
        {

        }

        public WSHTTPConnection(WSBuffers buffers) : base(buffers)
        {

        }

        static Regex regex = new Regex(@"\A(.+?): (.+)\Z", RegexOptions.Compiled);

        public Guid ID {
            get;
            protected set;
        } = Guid.NewGuid();

        public Task ConnectAsync(string url, CancellationToken cancellationToken = default)
        {
            if (url == null) {
                throw new ArgumentNullException(nameof(url));
            }

            var uri = new Uri(url);

            return ConnectAsync(uri, cancellationToken);
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri == null) {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = new TcpClient()) {
                await client.ConnectAsync(uri.Host, uri.Port);

                var clientStream = client.GetStream();

                var options = await ClientHandshake(clientStream, uri);

                await ProcessAsync(options, cancellationToken);
            }
        }

        public async Task ConnectAsync(TcpClient client, CancellationToken cancellationToken = default)
        {
            var clientStream = client.GetStream();

            var options = await ServerHandshake(clientStream);

            await ProcessAsync(options, cancellationToken);
        }

        protected virtual WSHTTPServerHeaders CreateServerHeaders()
        {
            return new WSHTTPServerHeaders();
        }

        protected virtual WSHTTPClientHeaders CreateClientHeaders()
        {
            return new WSHTTPClientHeaders();
        }

        protected virtual void UpdateHeaders(WSHTTPClientHeaders clientHeaders, WSHTTPServerHeaders serverHeaders)
        {
        }

        private async Task<IWSConnectionParameters> ServerHandshake(Stream stream)
        {
            var clientHeaders = CreateClientHeaders();
            var serverHeaders = CreateServerHeaders();

            var streamWriter = new StreamWriter(stream);
            var streamReader = new StreamReader(stream);

            var clientDirective = HTTPClientDirective.Parse(streamReader.ReadLine());
            if (clientDirective.Method != "GET" || clientDirective.PathAndQuery != "/ws") {
                throw new NotSupportedException("Client Handshake Request");
            }

            await ReadHeaders(streamReader, clientHeaders);

            var key = WSKey.FromString(clientHeaders["Sec-WebSocket-Key"]);

            ID = key.ID;

            UpdateHeaders(clientHeaders, serverHeaders);

            serverHeaders["Upgrade"] = "WebSocket";
            serverHeaders["Connection"] = "Upgrade";
            serverHeaders["Sec-WebSocket-Accept"] = key.ComputeAccept();

            var serverDirective = new HTTPServerDirective {
                Version = new Version(1, 1),
                StatusText = "Switch Protocol WebSocket",
                StatusCode = 101,
            };

            await SendResponse(serverHeaders, streamWriter, serverDirective);

            return new WSHTTPConnectionParameters(stream, clientHeaders, serverHeaders, WSHTTPConnectionType.Server);
        }

        private async Task<IWSConnectionParameters> ClientHandshake(Stream stream, Uri uri)
        {
            var clientHeaders = CreateClientHeaders();
            var serverHeaders = CreateServerHeaders();

            var streamWriter = new StreamWriter(stream);
            var streamReader = new StreamReader(stream);

            var key = WSKey.FromID(ID);

            clientHeaders["Host"] = uri.Host;
            clientHeaders["Upgrade"] = "WebSocket";
            clientHeaders["Connection"] = "Upgrade";
            clientHeaders["Sec-WebSocket-Key"] = key.Text;
            clientHeaders["Sec-WebSocket-Version"] = "13";

            var clientDirective = new HTTPClientDirective {
                Method = "GET",
                Version = new Version(1, 1),
                PathAndQuery = uri.PathAndQuery,
            };

            await SendResponse(clientHeaders, streamWriter, clientDirective);

            var serverDirective = HTTPServerDirective.Parse(streamReader.ReadLine());
            if (serverDirective.StatusCode != 101) {
                throw new NotSupportedException("Server Handshake Response");
            }

            await ReadHeaders(streamReader, serverHeaders);

            return new WSHTTPConnectionParameters(stream, clientHeaders, serverHeaders, WSHTTPConnectionType.Client);
        }

        private static async Task SendResponse(HTTPHeaders headers, StreamWriter streamWriter, IHTTPDirective directive)
        {
            streamWriter.WriteLine(directive.ToString());

            await SendHeaders(streamWriter, headers);

            streamWriter.Flush();
        }

        private static async Task SendHeaders(StreamWriter streamWriter, HTTPHeaders headers)
        {
            foreach (var key in headers) {
                if (string.IsNullOrEmpty(headers[key])) {
                    continue;
                }

                await streamWriter.WriteLineAsync($"{key}: {headers[key]}");
            }

            await streamWriter.WriteLineAsync();
        }

        private static async Task ReadHeaders(StreamReader streamReader, HTTPHeaders headers)
        {
            var text = default(string);
            do {
                text = await streamReader.ReadLineAsync();

                if (string.IsNullOrEmpty(text)) {
                    return;
                }

                var match = regex.Match(text);
                if (match.Success == false) {
                    throw new NotSupportedException(text);
                }

                var key = match.Groups[1].ToString();
                var val = match.Groups[2].ToString();

                if (headers.GetHeader(key) is IPushableHTTPHeader header) {
                    header.Push(val);
                }
                else {
                    headers[key] = val;
                }
            } while (text != null);
        }
    }
}
