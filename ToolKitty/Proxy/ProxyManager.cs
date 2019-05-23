using System.Collections.Generic;

namespace System.Reflection.Emit
{
    public class ProxyManager
    {
        static ProxyManager self;

        public static ProxyManager EnsureDefault()
        {
            if (self == null) {
                return self = new ProxyManager("ToolKitty.Proxies");
            }

            return self;
        }

        private readonly object
            threadLock = new object();
        private readonly Dictionary<ProxyKey, Type>
            proxyTypeCache = new Dictionary<ProxyKey, Type>();

        private readonly ModuleBuilder moduleBuilder;
        private readonly AssemblyBuilder assemblyBuilder;

        public ProxyManager(string name)
        {
            if (name == null) {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;

            var assemblyName = new AssemblyName(name);

            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(name);
        }

        public string Name { get; }

        public Type EnsureProxy(Type interfaceType)
        {
            return EnsureProxy(interfaceType, typeof(object));
        }

        public Type EnsureProxy(Type interfaceType, Type parentType)
        {
            if (parentType == null) {
                throw new ArgumentNullException(nameof(parentType));
            }

            if (interfaceType == null) {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            var key = new ProxyKey {
                InterfaceType = interfaceType,
                ParentType = parentType,
            };

            lock (threadLock) {
                if (proxyTypeCache.TryGetValue(key, out var proxyType) == false) {
                    proxyTypeCache[key] = proxyType = CreateProxy(key);
                }

                return proxyType;
            }
        }

        private Type CreateProxy(ProxyKey key)
        {
            var builder = new ProxyBuilder(key);

            return builder.Build(moduleBuilder);
        }
    }
}
