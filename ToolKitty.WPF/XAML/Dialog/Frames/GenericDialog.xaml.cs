using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ToolKitty.XAML
{
    /// <summary>
    /// Interaction logic for GenericDialog.xaml
    /// </summary>
    public partial class GenericDialog : Window
    {
        public static async Task ShowExceptionMessage(Exception exception)
        {
            var process = Process.GetCurrentProcess();
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Unhandled Error:");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(exception.Message);

            var model = new GenericDialogModel(stringBuilder.ToString(), process.ProcessName, MessageBoxButton.OK, MessageBoxImage.Error);

            await ShowDialogAsync(model);
        }

        public static async Task<MessageBoxResult> ShowDialogAsync(string message, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            var process = Process.GetCurrentProcess();

            var model = new GenericDialogModel(message, process.ProcessName, button, image);

            await ShowDialogAsync(model);

            return model.Result;
        }

        public static async Task<MessageBoxResult> ShowDialogAsync(string message, string caption, MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            var model = new GenericDialogModel(message, caption, button, image);

            await ShowDialogAsync(model);

            return model.Result;
        }

        public static Task<bool?> ShowDialogAsync(IDialogModel model)
        {
            return GO.Dispatch(delegate {
                var dialog = new GenericDialog {
                };

                dialog.NotifyWindowManager();

                return dialog.ShowDialogWithModelAsync(model);
            });
        }

        public static readonly DependencyProperty
            ModelProperty = DependencyProperty.Register(nameof(Model), typeof(IDialogModel), typeof(GenericDialog));

        public GenericDialog()
        {
            InitializeComponent();
        }

        public void NotifyWindowManager()
        {
            if (GO.GetService(typeof(IWindowManager)) is IWindowManager manager) {
                manager.NotifyWindow(this);
            }
        }

        public async Task<bool?> ShowDialogWithModelAsync(IDialogModel model)
        {
            if (model == null) {
                throw new ArgumentNullException(nameof(model), $"{nameof(model)} is null.");
            }

            Model = model;

            var result = await Dispatcher.InvokeAsync(ShowDialog);

            Model = null;

            return result;
        }

        public IDialogModel Model {
            get => GetValue(ModelProperty) as IDialogModel;
            set => SetValue(ModelProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            base.OnPropertyChanged(eventArgs);

            if (eventArgs.Property == ModelProperty) {
                UpdateModel(eventArgs);
            }
        }

        private async void Model_Complete(object sender, DialogCompleteEventArgs eventArgs)
        {
            if (Model is IDialogModel model) {
                model.Complete -= Model_Complete;
            }

            await Dispatcher.InvokeAsync(delegate {
                DialogResult = eventArgs.Result;

                Close();
            });
        }

        private void UpdateModel(DependencyPropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.OldValue is IDialogModel oldModel) {
                oldModel.Complete -= Model_Complete;
            }

            if (eventArgs.NewValue is IDialogModel newModel) {
                newModel.Complete += Model_Complete;
            }
        }
    }
}
