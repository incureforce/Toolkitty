using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace ToolKitty.Interop
{
    public static class KERNEL32
    {
        const string Library = nameof(KERNEL32);

        [DllImport(Library, SetLastError = true)]
        public static extern bool IsDebuggerPresent();
    }
}
