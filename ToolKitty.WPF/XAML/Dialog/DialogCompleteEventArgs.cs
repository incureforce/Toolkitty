using System;
using System.Linq;

namespace ToolKitty.XAML
{
    public class DialogCompleteEventArgs : EventArgs
    {
        public static readonly DialogCompleteEventArgs
            None = new DialogCompleteEventArgs(null),
            True = new DialogCompleteEventArgs(true),
            False = new DialogCompleteEventArgs(false);

        public DialogCompleteEventArgs(bool? result)
        {
            Result = result;
        }

        public bool? Result {
            get;
        }
    }
}
