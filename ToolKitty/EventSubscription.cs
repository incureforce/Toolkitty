using System;
using System.Linq;

namespace ToolKitty
{
    public class EventSubscription : Disposer
    {
        public EventSubscription(Delegate handler)
        {
            if (handler == null) {
                throw new ArgumentNullException(nameof(handler));
            }

            Handler = handler;
        }

        public Delegate Handler { get; }
    }
}
