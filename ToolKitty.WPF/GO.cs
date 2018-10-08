using System;
using System.Linq;
using System.Windows.Threading;

namespace ToolKitty
{
    public static class GO
    {
        private static bool ready = false;

        private static Dispatcher dispatcher;
        private static EventSystem eventSystem;
        private static ServiceManager serviceManager;

        public static void Initialize(Dispatcher dispatcher, EventSystem eventSystem = null, ServiceManager serviceManager = null)
        {
            if (dispatcher == null) {
                throw new ArgumentNullException(nameof(dispatcher));
            }

            ready = true;

            GO.dispatcher = dispatcher;
            GO.eventSystem = eventSystem ?? EventSystem.Default;
            GO.serviceManager = serviceManager ?? ServiceManager.Default;
        }

        public static void Publish<T>(T payload) where T : class
        {
            EnsureReady();

            eventSystem.Publish(payload);
        }

        public static IDisposable Subscribe(Action<object> subscribeHandler)
        {
            EnsureReady();

            return eventSystem.Subscribe(subscribeHandler);
        }

        public static IDisposable Subscribe<T>(Action<T> subscribeHandler) where T : class
        {
            EnsureReady();

            return eventSystem.Subscribe(subscribeHandler);
        }

        public static void SetService<T>(Func<T> serviceLocator, Type serviceType = null) where T : class
        {
            EnsureReady();

            serviceManager.SetService(serviceLocator, serviceType);
        }

        public static object GetService(Type serviceType)
        {
            EnsureReady();

            return serviceManager.GetService(serviceType);
        }

        public static void Dispatch(Action action)
        {
            EnsureReady();

            dispatcher.Invoke(action);
        }

        public static TResult Dispatch<TResult>(Func<TResult> action)
        {
            EnsureReady();

            return dispatcher.Invoke(action);
        }

        public static DispatcherOperation DispatchAsync(Action action)
        {
            EnsureReady();

            return dispatcher.InvokeAsync(action);
        }

        public static DispatcherOperation<TResult> DispatchAsync<TResult>(Func<TResult> action)
        {
            EnsureReady();

            return dispatcher.InvokeAsync(action);
        }

        private static void EnsureReady()
        {
            if (ready == false) {
                throw new InvalidOperationException($"Initialize hasn't been called yet");
            }
        }
    }
}
