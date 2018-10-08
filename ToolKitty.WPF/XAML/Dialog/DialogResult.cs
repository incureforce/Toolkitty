using System;
using System.Linq;

namespace TimeTracking.XAML
{
    public class DialogResult
    {
        public static readonly DialogResult
            Okay = new DialogResult(true),
            Fail = new DialogResult(false);

        public DialogResult(bool? result)
        {
            Result = result;
        }

        public bool? Result { get; }
    }
}
