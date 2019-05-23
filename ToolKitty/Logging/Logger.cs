using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ToolKitty.Logging
{
    public interface ILogger
    {
        void Log(Message message);
    }

    public interface IRelayLogger : ILogger, ICollection<ILogger>
    {
    }

    public class LoggerSetting
    {
        public static LoggerSetting Default
        {
            get;
        } = new LoggerSetting();

        /// <summary>
        /// {CREATED}{CHAR:TAB}{LEVEL:X}{CHAR:TAB}{TEXT}
        /// </summary>
        public string Format
        {
            get;
            set;
        } = "{CREATED}{CHAR:TAB}{LEVEL:X}{CHAR:TAB}{TEXT}";

        public MessageLevel MinLevel
        {
            get;
            set;
        } = MessageLevel.Default;

        public MessageLevel MaxLevel
        {
            get;
            set;
        } = MessageLevel.Exception;
    }

    public class DefaultRelayLogger : Collection<ILogger>, IRelayLogger
    {
        public void Log(Message message)
        {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }

            foreach (var logger in this.ToList()) {
                logger.Log(message);
            }
        }
    }

    public abstract class WriterLogger : LoggerSetting, ILogger
    {
        public WriterLogger()
        {
            var settings = Default;

            Format = settings.Format;
            MaxLevel = settings.MaxLevel;
            MinLevel = settings.MinLevel;
        }

        public void Log(Message message)
        {
            LogCore(message, message.ToString(Format, null));
        }

        protected abstract void LogCore(Message message, string text);
    }

    public enum MessageLevel
    {
        Debug,
        Trace,
        Default,
        Warning,
        Critical,
        Exception,
    }

    public static class Logger
    {
        public static Func<Formatters> DefaultFormatters
        {
            get;
            set;
        } = GetDefaultFormatters;

        public static IRelayLogger RelayLogger
        {
            get;
            set;
        } = new DefaultRelayLogger();

        static Formatters GetDefaultFormatters()
        {
            return Formatters.Default;
        }

        public static void AttachLogger(ILogger logger)
        {
            if (logger == null) {
                throw new ArgumentNullException(nameof(logger));
            }

            RelayLogger.Add(logger);
        }

        public static void RemoveLogger(ILogger logger)
        {
            if (logger == null) {
                throw new ArgumentNullException(nameof(logger));
            }

            RelayLogger.Remove(logger);
        }

        public static void LogDebug(string text)
        {
            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Debug));
        }

        public static void LogDebug(this ILogger self, string text)
        {
            self.Log(Message.CreateWithCollecting(text, MessageLevel.Debug));
        }

        public static void LogTrace(string text)
        {
            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Trace));
        }

        public static void LogTrace(this ILogger self, string text)
        {
            self.Log(Message.CreateWithCollecting(text, MessageLevel.Trace));
        }

        public static void LogDefault(string text)
        {
            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Default));
        }

        public static void LogDefault(this ILogger self, string text)
        {
            self.Log(Message.CreateWithCollecting(text, MessageLevel.Default));
        }

        public static void LogWarning(string text)
        {
            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Warning));
        }

        public static void LogWarning(this ILogger self, string text)
        {
            self.Log(Message.CreateWithCollecting(text, MessageLevel.Warning));
        }

        public static void LogCritical(string text)
        {
            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Critical));
        }

        public static void LogCritical(this ILogger self, string text)
        {
            self.Log(Message.CreateWithCollecting(text, MessageLevel.Critical));
        }

        public static void LogException(string text, Exception exception)
        {
            if (exception == null) {
                throw new ArgumentNullException(nameof(exception));
            }

            var info = GetExceptionText(exception);

            RelayLogger.Log(Message.CreateWithCollecting(text, MessageLevel.Exception));
            RelayLogger.Log(Message.CreateWithCollecting(info, MessageLevel.Trace));
        }

        public static void LogException(this ILogger self, string text, Exception exception)
        {
            if (exception == null) {
                throw new ArgumentNullException(nameof(exception));
            }

            var info = GetExceptionText(exception);

            self.Log(Message.CreateWithCollecting(text, MessageLevel.Exception));
            self.Log(Message.CreateWithCollecting(info, MessageLevel.Trace));
        }

        private static string GetExceptionText(Exception ex)
        {
            var index = -1;
            var stringBuilder = new StringBuilder();

            do {
                if (++index > 0) {
                    stringBuilder.AppendLine($"Inner Exception #{index}: {ex.Message}");
                }
                else {
                    stringBuilder.AppendLine(ex.Message);
                }

                stringBuilder.Append(ex.StackTrace);
            } while ((ex = ex.InnerException) != null);

            return stringBuilder.ToString();
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message message)
        {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
        }

        public Message Message { get; }
    }

    public class Message : Formatters, IFormattable
    {
        public static event EventHandler<MessageEventArgs> Collecting;

        static void RaiseCollecting(MessageEventArgs eventArgs)
        {
            if (eventArgs == null) {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            Collecting?.Invoke(null, eventArgs);
        }

        public static Message CreateWithCollecting(string message, MessageLevel level)
        {
            var entry = new Message(message, level);
            var entryEventArgs = new MessageEventArgs(entry);

            RaiseCollecting(entryEventArgs);

            return entry;
        }

        public Message(string text, MessageLevel level) : base(Logger.DefaultFormatters())
        {
            if (text == null) {
                throw new ArgumentNullException(nameof(text));
            }

            Text = text;
            Level = level;

            this[nameof(Text)] = text;
            this[nameof(Level)] = Level;
            this[nameof(Created)] = Created;
        }

        public string Text { get; }

        public DateTime Created { get; } = DateTime.UtcNow;

        public MessageLevel Level { get; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) {
                format = "{TEXT}";
            }

            return format.Format(this, formatProvider, false);
        }
    }

    public class ScopeTag
    {
        public const string Key = "Scope";

        public ScopeTag(string scope)
        {
            Scope = scope;
            if (string.IsNullOrEmpty(scope)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(scope));
            }
        }

        public string Scope { get; }

        public override string ToString()
        {
            return Scope;
        }
    }

    public class ScopeLogger : ILogger
    {
        private readonly ScopeTag scopeTag;

        public static ScopeLogger FromMember([CallerMemberName] string member = null)
        {
            return new ScopeLogger(member, Logger.RelayLogger);
        }

        public ScopeLogger(string scope, ILogger parent)
        {
            if (string.IsNullOrEmpty(scope)) {
                throw new ArgumentException("IsNullOrEmpty", nameof(scope));
            }

            if (parent == null) {
                throw new ArgumentNullException(nameof(parent));
            }

            scopeTag = new ScopeTag(scope);

            Scope = scope;
            Parent = parent;
        }

        public string Scope { get; }

        public ILogger Parent { get; }

        public void Log(Message message)
        {
            message[ScopeTag.Key] = scopeTag;

            Parent.Log(message);
        }
    }

    public class FileLogger : WriterLogger
    {
        public string Name
        {
            get;
            set;
        }

        protected override void LogCore(Message message, string text)
        {
            var formatters = Logger.DefaultFormatters();

            var file = Name.Format(formatters);

            using(var writer = File.AppendText(file)) {
                writer.WriteLine(text);
            }
        }
    }

    public class ConsoleLogger : WriterLogger
    {
        public bool UseColor { get; set; } = true;

        protected override void LogCore(Message message, string text)
        {
            if (UseColor) {
                var oldForeground = Console.ForegroundColor;
                var oldBackground = Console.BackgroundColor;

                Console.BackgroundColor = ConsoleColor.Black;

                switch (message.Level) {
                    case MessageLevel.Debug:
                        Console.ForegroundColor = (ConsoleColor)09;
                        break;
                    case MessageLevel.Trace:
                        Console.ForegroundColor = (ConsoleColor)10;
                        break;
                    case MessageLevel.Default:
                        Console.ForegroundColor = (ConsoleColor)15;
                        break;
                    case MessageLevel.Warning:
                        Console.ForegroundColor = (ConsoleColor)14;
                        break;
                    case MessageLevel.Critical:
                        Console.ForegroundColor = (ConsoleColor)06;
                        break;
                    case MessageLevel.Exception:
                        Console.ForegroundColor = (ConsoleColor)12;
                        break;
                }

                Console.WriteLine(text);

                Console.ForegroundColor = oldForeground;
                Console.BackgroundColor = oldBackground;
            } else {
                Console.WriteLine(text);
            }
        }
    }
}
