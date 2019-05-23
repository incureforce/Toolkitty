using System.Collections.Generic;
using System.Text;

namespace ToolKitty.WebSockets
{
    internal struct WSFrameHeader
    {
        public bool Last;
        public bool Mask;
        public bool RSV0;
        public bool RSV1;
        public bool RSV2;

        public int Code;
        public int Length;
    }
}
