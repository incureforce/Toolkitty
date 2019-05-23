using System.Buffers;

namespace ToolKitty.WebSockets
{
    public struct WSFrame
    {
        public int Code;
        public bool Last;
        public IFragment<byte> Fragment;
    }
}
