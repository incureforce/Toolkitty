using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.ComponentModel
{
    public class UIBindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs == null) {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            PropertyChanged?.Invoke(this, eventArgs);
        }

        protected void RaisePropertyChanged([CallerMemberName] string member = null)
        {
            if (member == null) {
                throw new ArgumentNullException(nameof(member));
            }

            RaisePropertyChanged(new PropertyChangedEventArgs(member));
        }

        protected bool RaisePropertyChangedWhen<T>(ref T field, T value, [CallerMemberName] string member = null)
        {
            if (member == null) {
                throw new ArgumentNullException(nameof(member));
            }

            var comparer = EqualityComparer<T>.Default;

            if (comparer.Equals(field, value)) {
                return false;
            }

            field = value;

            RaisePropertyChanged(new PropertyChangedEventArgs(member));

            return true;
        }
    }
}
