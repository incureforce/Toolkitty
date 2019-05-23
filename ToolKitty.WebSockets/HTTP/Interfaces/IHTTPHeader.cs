using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public interface IHTTPHeader
    {
        string Get();
        string Set(string value);
    }
}
