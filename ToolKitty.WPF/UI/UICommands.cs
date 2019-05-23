using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;

namespace System.ComponentModel
{
    public class UICommands : DynamicObject, IReadOnlyCollection<UICommand>
    {
        private readonly Dictionary<string, UICommand>
            commandMap = new Dictionary<string, UICommand>(StringComparer.OrdinalIgnoreCase);

        public int Count => commandMap.Count;

        public UICommand this[string code]
        {
            get => commandMap[code];
            set => commandMap[code] = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (commandMap.TryGetValue(binder.Name, out var command)) {
                result = command;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public IEnumerator<UICommand> GetEnumerator()
        {
            return commandMap.Values
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return commandMap.Values
                .GetEnumerator();
        }

        public void Refresh()
        {
            foreach (var command in commandMap.Values) {
                command.RaiseCanExecuteChanged(EventArgs.Empty);
            }
        }

        public void Refresh(string code)
        {
            var command = commandMap[code];

            command.RaiseCanExecuteChanged(EventArgs.Empty);
        }

        public void Execute(string code, object parameter)
        {
            var command = commandMap[code];

            command.Execute(parameter);
        }
    }
}
