using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public interface IWSMessage
    {
        int Code { get; }
    }
}
