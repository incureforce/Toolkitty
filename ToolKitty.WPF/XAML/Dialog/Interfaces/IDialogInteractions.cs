using System;
using System.ComponentModel;
using System.Linq;

namespace TimeTracking.XAML
{
    public interface IDialogInteractions : IModelInteractions
    {
        string Title { get; }

        event EventHandler<DialogResult> Callback;
    }
}
