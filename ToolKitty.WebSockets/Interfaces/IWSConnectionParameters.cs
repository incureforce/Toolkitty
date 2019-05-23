using System;
using System.IO;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public interface IWSConnectionParameters
    {
        Stream Stream { get; }

        TimeSpan Heartbeat { get; }
    }
}
