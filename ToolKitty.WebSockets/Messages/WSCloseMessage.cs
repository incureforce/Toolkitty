using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public class WSCloseMessage : IWSMessage
    {
        public int Code => WSConsts.CodeClose;

        public ushort StatusCode {
            get;
            set;
        }

        public string StatusText {
            get;
            set;
        }
    }
}
