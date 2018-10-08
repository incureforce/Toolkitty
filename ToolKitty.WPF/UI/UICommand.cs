using System.ComponentModel;

namespace System.Windows.Input
{
    public class UICommand : UIBindable, ICommand
    {
        public delegate void ExecuteHandler(object parameter);
        public delegate bool EnabledHandler(object parameter);

        private object icon;
        private object header;
        private object description;

        private readonly ExecuteHandler execute;
        private readonly EnabledHandler enabled;

        public UICommand(ExecuteHandler executeHandler, EnabledHandler enabledHandler = null)
        {
            if (executeHandler == null) {
                throw new ArgumentNullException(nameof(executeHandler));
            }

            execute = executeHandler;
            enabled = enabledHandler ?? DefaultEnabledHandler;
        }

        public event EventHandler CanExecuteChanged;

        public object Icon
        {
            get => icon;
            set => RaisePropertyChangedWhen(ref icon, value);
        }

        public object Header
        {
            get => header;
            set => RaisePropertyChangedWhen(ref header, value);
        }

        public object Description
        {
            get => description;
            set => RaisePropertyChangedWhen(ref description, value);
        }

        public bool CanExecute(object parameter)
        {
            return enabled(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);

            RaiseCanExecuteChanged(EventArgs.Empty);
        }

        public void RaiseCanExecuteChanged(EventArgs eventArgs)
        {
            if (eventArgs == null) {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if (enabled == DefaultEnabledHandler) {
                return;
            }

            CanExecuteChanged?.Invoke(this, eventArgs);
        }

        private static bool DefaultEnabledHandler(object parameter)
        {
            return true;
        }
    }
}
