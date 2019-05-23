using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Windows.Input
{
    public class UICommand : UIBindable, ICommand
    {
        public delegate Task ExecuteHandler(object parameter);
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

        public virtual event EventHandler CanExecuteChanged;

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

        public async void Execute(object parameter)
        {
            await execute(parameter);

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

    public class WPFUICommand : UICommand
    {
        public static WPFUICommand Create(ExecuteHandler executeHandler, EnabledHandler enabledHandler = null)
        {
            var method = executeHandler.Method;

            return new WPFUICommand(executeHandler, enabledHandler) {
                Icon = GlyphIcons.GetIconModel(method),
                Header = Translator.GetTranslation(method, TranslationIntent.DisplayName, true),
                Description = Translator.GetTranslation(method, TranslationIntent.Description, false),
            };
        }

        public WPFUICommand(ExecuteHandler executeHandler, EnabledHandler enabledHandler = null) : base(executeHandler, enabledHandler)
        {
        }

        public override event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
