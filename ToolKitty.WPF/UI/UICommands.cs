using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows.Input;

namespace System.ComponentModel
{
    public class UICommands : DynamicObject, IReadOnlyCollection<UICommand>
    {
        private readonly Dictionary<Enum, UICommand>
            commandMap = new Dictionary<Enum, UICommand>();

        public int Count => commandMap.Count;

        public UICommand this[Enum code]
        {
            get => commandMap[code];
            set => commandMap[code] = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            foreach (var pair in commandMap) {
                if (string.Equals($"{pair.Key}", binder.Name, StringComparison.OrdinalIgnoreCase)) {
                    result = pair.Value;

                    return true;
                }
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

        public void Refresh(Enum code)
        {
            var command = commandMap[code];

            command.RaiseCanExecuteChanged(EventArgs.Empty);
        }

        public void Execute(Enum code, object parameter)
        {
            var command = commandMap[code];

            command.Execute(parameter);
        }
    }
}
