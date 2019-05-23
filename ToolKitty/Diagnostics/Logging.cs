using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolKitty.Diagnostics
{

    public static class Logging
    {
        public static RelayLogger RelayLogger {
            get;
        } = new RelayLogger();

        public static void Debug(string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Debug, message, arg);

            RelayLogger.Log(log);
        }

        public static void Trace(string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Trace, message, arg);

            RelayLogger.Log(log);
        }

        public static void Default(string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Default, message, arg);

            RelayLogger.Log(log);
        }

        public static void Warning(string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Warning, message, arg);

            RelayLogger.Log(log);
        }

        public static void Critical(string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Critical, message, arg);

            RelayLogger.Log(log);
        }

        public static void Exception(string message, Exception exception, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Exception, message, arg);

            log[nameof(Exception)] = exception;

            RelayLogger.Log(log);
        }

        public static void Debug(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Debug, message, arg);

            logger.Log(log);
        }

        public static void Trace(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Trace, message, arg);

            logger.Log(log);
        }

        public static void Default(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Default, message, arg);

            logger.Log(log);
        }

        public static void Warning(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Warning, message, arg);

            logger.Log(log);
        }

        public static void Critical(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Critical, message, arg);

            logger.Log(log);
        }

        public static void Exception(this ILogger logger, string message, object arg = null)
        {
            var log = ApplyRuntimeInfos(LogLevel.Exception, message, arg);

            logger.Log(log);
        }

        private static Log ApplyRuntimeInfos(LogLevel level, string message, object arg)
        {
            var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (arg != null) {
                if (arg is IDictionary<string, object> map) {
                    foreach (var key in map.Keys) {
                        dictionary[key] = map[key];
                    }
                }
                else {
                    var type = arg.GetType();

                    foreach (var property in type.GetProperties()) {
                        dictionary[property.Name] = property.GetValue(arg);
                    }
                }
            }

            return Log.Create(level, message.Format(dictionary), dictionary);
        }
    }
}
