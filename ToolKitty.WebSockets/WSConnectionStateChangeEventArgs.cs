using System;

namespace ToolKitty.WebSockets
{
    public class WSConnectionStateChangeEventArgs : EventArgs
    {
        public WSConnectionStateChangeEventArgs(bool connected)
        {
            IsConnected = connected;
        }

        public bool IsConnected {
            get;
        }
    }
}
