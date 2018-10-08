using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolKitty
{
    public class EventSystem
    {
        public static EventSystem Default
        {
            get;
        } = new EventSystem();

        private readonly Dictionary<Delegate, Disposer>
            subscriberMap = new Dictionary<Delegate, Disposer>();

        public IDisposable Subscribe<T>(Action<T> subscribeHandler) where T : class
        {
            if (subscriberMap.TryGetValue(subscribeHandler, out var disposer) == false) {
                var subscription = new EventSubscription(subscribeHandler);

                subscription.Disposing += Subscription_Disposing;

                subscriberMap[subscribeHandler] = subscription;
            }

            return disposer;
        }

        private void Subscription_Disposing(object sender, EventArgs e)
        {
            var subscription = (EventSubscription)sender;

            subscription.Disposing -= Subscription_Disposing;

            subscriberMap.Remove(subscription.Handler);
        }

        public void Publish<T>(T payload) where T : class
        {
            if (payload == null) {
                throw new ArgumentNullException(nameof(payload));
            }

            foreach (var subscribeHandler in Enumerable.ToList(subscriberMap.Keys)) {
                if (subscribeHandler is Action<T> action) {
                    action(payload);
                }
            }
        }
    }
}
