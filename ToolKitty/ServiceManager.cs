using System;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public class ServiceManager : IServiceProvider
    {
        private readonly Dictionary<Type, Delegate>
            providerMap = new Dictionary<Type, Delegate>();

        public ServiceManager()
        { }

        public ServiceManager(IServiceProvider parent)
        {
            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public IServiceProvider Parent { get; }

        public IEnumerable<Type> Types => providerMap.Keys;

        public static ServiceManager Default
        {
            get;
        } = new ServiceManager();

        public void Clear()
        {
            providerMap.Clear();
        }

        public void Remove(Type type)
        {
            providerMap.Remove(type);
        }

        public void SetService<T>(Func<T> serviceLocator, Type serviceType = null) where T : class
        {
            if (serviceType == null) {
                serviceType = typeof(T);
            }

            providerMap[serviceType] = serviceLocator;
        }

        public object GetService(Type serviceType)
        {
            if (providerMap.TryGetValue(serviceType, out var serviceLocator)) {
                return serviceLocator.DynamicInvoke();
            }

            if (Parent is IServiceProvider serviceProvider) {
                return serviceProvider.GetService(serviceType);
            }

            return null;
        }
    }
}
