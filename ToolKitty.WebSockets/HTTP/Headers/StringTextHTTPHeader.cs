using System;
using System.Linq;

namespace ToolKitty.WebSockets
{
    public class StringTextHTTPHeader : IHTTPHeader
    {
        public string Text {
            get;
            set;
        }

        public string Get()
        {
            return Text;
        }

        public string Set(string value)
        {
            return Text = value;
        }
    }
}
