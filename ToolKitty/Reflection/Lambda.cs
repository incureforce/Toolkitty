using System;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection
{
    public class Lambda
    {
        delegate object[] Collector();

        public static Lambda Compose(LambdaExpression expression)
        {
            if (expression == null) {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!(expression.Body is MethodCallExpression methodCall)) {
                throw new NotSupportedException($"Body is no Method Call");
            }

            var args = methodCall.Arguments
                .Select(arg => Expression.Convert(arg, typeof(object)));

            var lambdaBody = Expression.NewArrayInit(typeof(object), args);

            var lambda = Expression.Lambda<Collector>(lambdaBody);

            var collector = lambda.Compile();

            return new Lambda(methodCall.Method, collector());
        }

        public Lambda(MethodInfo methodInfo, object[] parameters)
        {
            if (methodInfo == null) {
                throw new ArgumentNullException(nameof(methodInfo));
            }

            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }

            MethodInfo = methodInfo;
            Parameters = parameters;
        }

        public MethodInfo MethodInfo { get; }

        public object[] Parameters { get; }

        public T[] CreateParameters<T>(Func<ParameterInfo, object, T> func) where T : IParameterInfo
        {
            if (func == null) {
                throw new ArgumentNullException(nameof(func));
            }

            var parameterInfos = MethodInfo.GetParameters();
            var length = Parameters.Length;
            var array = new T[length];

            for (var i = 0; i < length; ++i) {
                array[i] = func(parameterInfos[i], Parameters[i]);
            }

            return array;
        }
    }
}
