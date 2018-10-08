using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TimeTracking.XAML
{
    public class Dialog
    {
        public static Task<bool?> ShowDialogAsync(Window window, IDialogInteractions interactions)
        {
            var dialog = new Dialog(window, interactions);

            return dialog.ShowDialogAsync();
        }

        public Dialog(Window window, IDialogInteractions interactions)
        {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }

            if (interactions == null) {
                throw new ArgumentNullException(nameof(interactions));
            }

            Window = window;
            Interactions = interactions;
        }

        public Window Window { get; }

        public IDialogInteractions Interactions { get; }

        public async Task<bool?> ShowDialogAsync()
        {
            var dispatcher = Window.Dispatcher;

            Interactions.Callback += Interactions_Callback;

            Window.DataContext = Interactions;

            var result = await dispatcher.InvokeAsync(Window.ShowDialog);

            Window.DataContext = null;

            Interactions.Callback -= Interactions_Callback;

            return result;
        }

        private void Interactions_Callback(object sender, DialogResult eventArgs)
        {
            Window.DialogResult = eventArgs.Result;

            Window.Close();
        }
    }
}
