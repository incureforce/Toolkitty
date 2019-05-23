using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ToolKitty.XAML
{
    public interface IDialogModel
    {
        event EventHandler<DialogCompleteEventArgs> Complete;

        object Header { get; }

        object Content { get; }

        IEnumerable<IDialogButton> Buttons { get; }
    }
}
