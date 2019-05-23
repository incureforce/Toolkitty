using System;
using System.Buffers;
using System.IO;

namespace ToolKitty.WebSockets
{
    public struct WSConnectionParameters : IWSConnectionParameters
    {
        public Stream Stream { get; internal set; }

        public TimeSpan Heartbeat { get; internal set; }

        public bool IsIncomplete {
            get => Stream == null;
        }

        public WSConnectionParameters CloneWithStream(Stream stream)
        {
            return new WSConnectionParameters {
                Stream = stream,

                Heartbeat = Heartbeat,
            };
        }
    }
}
