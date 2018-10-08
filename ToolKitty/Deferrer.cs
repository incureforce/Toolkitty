using System;
using System.Linq;
using System.Threading;

namespace System
{
    public class Deferrer
    {
        public static Deferrer WithLock(EventHandler handler)
        {
            var deferrer = new Deferrer();

            deferrer.Locking += handler;

            return deferrer;
        }

        public static Deferrer WithFree(EventHandler handler)
        {
            var deferrer = new Deferrer();

            deferrer.Freeing += handler;

            return deferrer;
        }

        public static Deferrer WithBoth(EventHandler lockHandler, EventHandler freeHandler)
        {
            var deferrer = new Deferrer();

            deferrer.Locking += lockHandler;
            deferrer.Freeing += freeHandler;

            return deferrer;
        }

        private int counter = 0;

        public event EventHandler Locking;
        public event EventHandler Freeing;

        public bool IsLocked
        {
            get => counter > 0;
        }

        public Disposer Defer()
        {
            if (Interlocked.Increment(ref counter) == 1) {
                OnLocking(EventArgs.Empty);
            }

            var disposer = CreateDisposer();

            disposer.Disposing += Disposer_Disposing;

            return disposer;
        }

        protected virtual void OnLocking(EventArgs eventArgs)
        {
            RaiseLocking(eventArgs);
        }

        protected virtual void OnFreeing(EventArgs eventArgs)
        {
            RaiseFreeing(eventArgs);
        }

        protected virtual Disposer CreateDisposer()
        {
            return new Disposer();
        }

        private void Disposer_Disposing(object sender, EventArgs e)
        {
            var disposer = (Disposer)sender;

            disposer.Disposing -= Disposer_Disposing;

            if (Interlocked.Decrement(ref counter) == 0) {
                OnFreeing(EventArgs.Empty);
            }
        }

        private void RaiseLocking(EventArgs empty)
        {
            if (empty == null) {
                throw new ArgumentNullException(nameof(empty));
            }

            Locking?.Invoke(this, empty);
        }

        private void RaiseFreeing(EventArgs empty)
        {
            if (empty == null) {
                throw new ArgumentNullException(nameof(empty));
            }

            Freeing?.Invoke(this, empty);
        }
    }
}
