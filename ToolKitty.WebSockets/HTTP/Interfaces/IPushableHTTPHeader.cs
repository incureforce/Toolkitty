using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public interface IPushableHTTPHeader : IHTTPHeader
    {
        void Push(string value);
    }
}
