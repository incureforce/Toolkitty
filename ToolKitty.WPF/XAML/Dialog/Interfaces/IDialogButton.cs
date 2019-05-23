using System;
using System.Linq;
using System.Windows.Input;

namespace ToolKitty.XAML
{
    public interface IDialogButton : ICommand
    {
        bool IsCancel { get; }

        bool IsDefault { get; }

        object Content { get; }

        object ToolTip { get; }
    }
}
