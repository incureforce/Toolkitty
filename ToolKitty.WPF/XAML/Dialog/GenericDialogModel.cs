using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ToolKitty.XAML
{
    internal class GenericDialogModel : UIBindable, IDialogModel
    {
        private readonly GenericDialogButton
            button0,
            button1,
            button2,
            button3;

        private readonly GenericDialogButton[]
            buttonGroup0,
            buttonGroup1,
            buttonGroup2,
            buttonGroup3;

        public GenericDialogModel(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            button0 = new GenericDialogButton(Button0_Execute) {
                IsDefault = true,
                Content = "OK",
            };

            button1 = new GenericDialogButton(Button1_Execute) {
                IsCancel = true,
                Content = "Cancel",
            };

            button2 = new GenericDialogButton(Button2_Execute) {
                Content = "Yes",
            };

            button3 = new GenericDialogButton(Button3_Execute) {
                Content = "No",
            };

            Button = button;
            buttonGroup0 = new[] { button0 };
            buttonGroup1 = new[] { button0, button1 };
            buttonGroup2 = new[] { button2, button3 };
            buttonGroup3 = new[] { button2, button3, button1 };

            Header = caption;
            Content = message;
        }

        public event EventHandler<DialogCompleteEventArgs> Complete;

        public object Header {
            get;
        }

        public object Content {
            get;
        }

        public MessageBoxButton Button {
            get;
        }

        public IEnumerable<IDialogButton> Buttons {
            get {
                switch (Button) {
                    case MessageBoxButton.OK:
                        return buttonGroup0;
                    case MessageBoxButton.OKCancel:
                        return buttonGroup1;
                    case MessageBoxButton.YesNoCancel:
                        return buttonGroup2;
                    case MessageBoxButton.YesNo:
                        return buttonGroup3;
                    default: throw new NotSupportedException($"{Button}");
                }
            }
        }

        public MessageBoxResult Result {
            get;
            private set;
        }

        protected virtual void RaiseComplete(DialogCompleteEventArgs eventArgs)
        {
            Complete?.Invoke(this, eventArgs);
        }

        public Task Button0_Execute(object parameter)
        {
            Result = MessageBoxResult.OK;

            RaiseComplete(DialogCompleteEventArgs.True);

            return Task.CompletedTask;
        }

        public Task Button1_Execute(object parameter)
        {
            Result = MessageBoxResult.Cancel;

            RaiseComplete(DialogCompleteEventArgs.None);

            return Task.CompletedTask;
        }

        public Task Button2_Execute(object parameter)
        {
            Result = MessageBoxResult.Yes;

            RaiseComplete(DialogCompleteEventArgs.True);

            return Task.CompletedTask;
        }

        public Task Button3_Execute(object parameter)
        {
            Result = MessageBoxResult.No;

            RaiseComplete(DialogCompleteEventArgs.False);

            return Task.CompletedTask;
        }
    }
}
