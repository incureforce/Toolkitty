using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToolKitty.Diagnostics
{
    public enum ContextScope
    {
        Type = 1,
        Method = 2,
        Namespace = 4,
        NamespaceWithType = Namespace | Type,
    }

    public class ContextLogger : Formatters, ILogger
    {
        public static ContextLogger FromCallStack(ContextScope scope = ContextScope.Namespace, ILogger logger = null) {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);

            var method = stackFrame.GetMethod();

            return FromScope(GetScope(scope, method), logger);
        }

        private static string GetScope(ContextScope scope, MethodBase method) {
            var stringBuilder = new StringBuilder();
            var declaringType = method.DeclaringType;

            switch (scope) {
                case ContextScope.Type:
                    return declaringType.Name;
                case ContextScope.Namespace:
                    return declaringType.Namespace;
                case ContextScope.NamespaceWithType:
                    return declaringType.ToString();
                default: throw new NotSupportedException($"{scope}");
            }
        }

        public static ContextLogger FromScope([CallerMemberName] string member = null, ILogger logger = null) {
            return new ContextLogger(logger ?? Logging.RelayLogger) {
                ["Member"] = member,
            };
        }

        public ContextLogger(ILogger logger)
        {
            if (logger == null) {
                throw new ArgumentNullException(nameof(logger));
            }

            Logger = logger;
        }

        public ILogger Logger {
            get;
        }

        public void Log(Log log)
        {
            foreach (var key in Keys) {
                log[key] = this[key];
            }

            Logger.Log(log);
        }
    }
}
