using System.Collections.ObjectModel;

namespace System.Reflection.Emit
{
    internal class ProxyMethodMapping : Collection<MethodInfo>
    {
        public ProxyMethodMapping(MethodBuilder methodBuilder, Type[] methodParameterTypes)
        {
            if (methodBuilder == null) {
                throw new ArgumentNullException(nameof(methodBuilder));
            }

            if (methodParameterTypes == null) {
                throw new ArgumentNullException(nameof(methodParameterTypes));
            }

            MethodBuilder = methodBuilder;
            MethodParameterTypes = methodParameterTypes;
        }

        public string Name {
            get => MethodBuilder.Name;
        }

        public Type[] MethodParameterTypes {
            get;
        }

        public MethodBuilder MethodBuilder {
            get;
        }
    }
}
