using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKitty.WebSockets
{
    public class WSTextMessage : IWSMessage
    {
        public int Code => WSConsts.CodeContentText;

        public string Text {
            get;
            set;
        }
    }
}
