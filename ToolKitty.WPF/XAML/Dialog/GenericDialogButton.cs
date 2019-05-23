using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ToolKitty.XAML
{
    public class GenericDialogButton : UIBindable, IDialogButton
    {
        public delegate Task ExecuteHandler(object parameter);
        public delegate bool EnabledHandler(object parameter);

        private bool
            isCancel,
            isDefault;
        private object
            content,
            toolTip;
        private readonly ExecuteHandler ExecuteCore;
        private readonly EnabledHandler EnabledCore;

        public event EventHandler CanExecuteChanged;

        public GenericDialogButton(ExecuteHandler execute)
        {
            if (execute == null) {
                throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} is null.");
            }

            ExecuteCore = execute;
            EnabledCore = CanExecuteFallback;
        }

        public GenericDialogButton(ExecuteHandler execute, EnabledHandler enabled = null)
        {
            if (execute == null) {
                throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} is null.");
            }

            ExecuteCore = execute;
            EnabledCore = enabled ?? CanExecuteFallback;
        }

        public bool IsCancel {
            get => isCancel;
            set => RaisePropertyChangedWhen(ref isCancel, value);
        }

        public bool IsDefault {
            get => isDefault;
            set => RaisePropertyChangedWhen(ref isDefault, value);
        }

        public object Content {
            get => content;
            set => RaisePropertyChangedWhen(ref content, value);
        }

        public object ToolTip {
            get => toolTip;
            set => RaisePropertyChangedWhen(ref toolTip, value);
        }

        public bool CanExecute(object parameter)
        {
            if (EnabledCore == CanExecuteFallback) {
                return true;
            }

            return EnabledCore(parameter);
        }

        public async void Execute(object parameter)
        {
            await ExecuteCore(parameter);

            RaiseCanExecuteChanged(EventArgs.Empty);
        }

        public virtual void RaiseCanExecuteChanged(EventArgs eventArgs)
        {
            if (eventArgs == null) {
                throw new ArgumentNullException(nameof(eventArgs), $"{nameof(eventArgs)} is null.");
            }

            if (EnabledCore == CanExecuteFallback) {
                return;
            }

            CanExecuteChanged?.Invoke(this, eventArgs);
        }

        private static bool CanExecuteFallback(object paramemter) => true;
    }
}
