using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Reflection.Emit
{
    internal class ProxyBuilder
    {
        private const MethodAttributes
            ProxyConstructorAttributes = MethodAttributes.Public,
            ProxyMethodAttributes = MethodAttributes.Virtual | MethodAttributes.Public;
        private const FieldAttributes 
            ProxyFieldAttributes = FieldAttributes.Private | FieldAttributes.InitOnly;
        private const TypeAttributes
            ProxyTypeAttributes = TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.Class;
        private const string 
            FieldName = "__<>translator";

        private static readonly Type
            TypeOfTask = typeof(Task),
            TypeOfMethodBase = typeof(MethodBase),
            TypeOfIProxyIntercepter = typeof(IProxyInterceptor);

        private Type parentType;
        private Type[] interfaceTypes;

        private readonly Dictionary<string, ProxyFieldMapping>
            fieldMappingMap = new Dictionary<string, ProxyFieldMapping>();
        private readonly Dictionary<string, ProxyMethodMapping>
            methodMappingMap = new Dictionary<string, ProxyMethodMapping>();

        public ProxyBuilder(ProxyKey key)
        {
            parentType = key.ParentType;
            interfaceTypes = new Type[] {
                key.InterfaceType
            };
        }

        public Type Build(ModuleBuilder moduleBuilder)
        {
            var typeBuilder = moduleBuilder.DefineType($"proxy${parentType}", ProxyTypeAttributes, parentType, interfaceTypes);

            DefineField(typeBuilder);

            DefineMethodWithInterfaces(typeBuilder);

            DefineConstructor(typeBuilder);

            return typeBuilder.CreateType();
        }

        private void DefineConstructor(TypeBuilder typeBuilder)
        {
            foreach (var constructor in parentType.GetConstructors()) {
                var constructorParameters = constructor.GetParameters();
                var constructorParameterTypes = new Type[constructorParameters.Length + 1];

                constructorParameterTypes[0] = TypeOfIProxyIntercepter;

                for (var i = 0; i < constructorParameters.Length; ++i) {
                    constructorParameterTypes[i + 1] = constructorParameters[i].ParameterType;
                }

                var constuctorBuilder = typeBuilder.DefineConstructor(ProxyConstructorAttributes, CallingConventions.HasThis, constructorParameterTypes);

                var ilGenerator = constuctorBuilder.GetILGenerator();

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Dup);

                for (var i = 0; i < constructorParameters.Length; ++i) {
                    ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                }

                ilGenerator.Emit(OpCodes.Call, constructor);

                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Stfld, fieldMappingMap[FieldName].FieldBuilder);
                ilGenerator.Emit(OpCodes.Ret);
            }
        }

        private void DefineField(TypeBuilder typeBuilder)
        {
            var fieldBuilder = typeBuilder.DefineField(FieldName, TypeOfIProxyIntercepter, ProxyFieldAttributes);

            fieldMappingMap[fieldBuilder.Name] = new ProxyFieldMapping(fieldBuilder);
        }

        private void DefineMethodWithInterfaces(TypeBuilder typeBuilder)
        {
            var interfaceList = new List<Type>(interfaceTypes);

            if (interfaceList.Any()) {
                var i = 0;

                for (var interfaceType = interfaceList[0]; i < interfaceList.Count; ++i) {
                    interfaceType = interfaceList[i];

                    typeBuilder.AddInterfaceImplementation(interfaceType);

                    foreach (var interfaceMethod in interfaceType.GetMethods()) {
                        if (methodMappingMap.TryGetValue(interfaceMethod.Name, out var methodMapping) == false) {
                            methodMappingMap[interfaceMethod.Name] = methodMapping = DefineMethod(typeBuilder, interfaceMethod);
                        }

                        methodMapping.Insert(methodMapping.Count, interfaceMethod);

                        typeBuilder.DefineMethodOverride(methodMapping.MethodBuilder, interfaceMethod);
                    }

                    foreach (var nestedInterfaceType in interfaceType.GetInterfaces()) {
                        if (interfaceList.Contains(nestedInterfaceType)) {
                            continue;
                        }

                        interfaceList.Insert(interfaceList.Count, nestedInterfaceType);
                    }
                }
            }
        }

        private ProxyMethodMapping DefineMethod(TypeBuilder typeBuilder, MethodInfo method)
        {
            var methodParameters = method.GetParameters();
            var methodParameterTypes = new Type[methodParameters.Length];

            for (var i = 0; i < methodParameters.Length; ++i) {
                methodParameterTypes[i] = methodParameters[i].ParameterType;
            }

            var methodReturnType = method.ReturnType;
            var methodBuilder = typeBuilder.DefineMethod(method.Name, ProxyMethodAttributes, methodReturnType, methodParameterTypes);

            var method1 = TypeOfMethodBase.GetMethod(nameof(MethodBase.GetCurrentMethod));
            var method2 = TypeOfIProxyIntercepter.GetMethod(nameof(IProxyInterceptor.InvokeAsync));

            var ilGenerator = methodBuilder.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldfld, fieldMappingMap[FieldName].FieldBuilder);

            ilGenerator.EmitCall(OpCodes.Call, method1, null);

            ilGenerator.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
            ilGenerator.Emit(OpCodes.Newarr, typeof(object));

            for (var i = 0; i < methodParameterTypes.Length; ++i) {
                ilGenerator.Emit(OpCodes.Dup);
                ilGenerator.Emit(OpCodes.Ldarg, i + 1);
                ilGenerator.Emit(OpCodes.Ldc_I4, i + 0);

                if (methodParameterTypes[i].IsValueType) {
                    ilGenerator.Emit(OpCodes.Box, methodParameterTypes[i]);
                }

                ilGenerator.Emit(OpCodes.Stelem, typeof(object));
            }

            ilGenerator.EmitCall(OpCodes.Call, method2, null);

            if (methodReturnType.Equals(TypeOfTask) || methodReturnType.IsSubclassOf(TypeOfTask)) {
                if (methodReturnType.Equals(TypeOfTask)) {
                    ilGenerator.Emit(OpCodes.Castclass, methodReturnType);
                }

                ilGenerator.Emit(OpCodes.Ret);
            }
            else {
                throw new NotSupportedException($"Only Task return types supported on '{method}'");
            }

            return new ProxyMethodMapping(methodBuilder, methodParameterTypes);
        }
    }
}
