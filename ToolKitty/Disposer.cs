using System;
using System.Linq;

namespace System
{
    public class Disposer : IDisposable
    {
        public static Disposer Use(EventHandler disposingHandler)
        {
            var disposer = new Disposer();

            disposer.Disposing += disposingHandler;

            return disposer;
        }

        public event EventHandler Disposing;

        public void Dispose()
        {
            OnDisposing(EventArgs.Empty);
        }

        protected virtual void OnDisposing(EventArgs eventArgs)
        {
            RaiseDisposing(eventArgs);
        }

        private void RaiseDisposing(EventArgs empty)
        {
            if (empty == null) {
                throw new ArgumentNullException(nameof(empty));
            }

            Disposing?.Invoke(this, empty);
        }
    }
}
