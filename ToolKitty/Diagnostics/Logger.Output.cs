using System.IO;
using System.Threading;

namespace ToolKitty.Diagnostics
{
    public abstract class OutputLogger : ILogger
    {
        public LogLevel MinLevel {
            get;
            set;
        } = LogLevel.Default;

        public LogLevel MaxLevel {
            get;
            set;
        } = LogLevel.Exception;

        public string Format {
            get;
            set;
        } = "{message}";


        public void Log(Log log)
        {
            if (log.Level >= MinLevel && MaxLevel >= log.Level) {
                OnLog(log, log.ToString(Format, Thread.CurrentThread.CurrentUICulture));
            }
        }

        protected abstract void OnLog(Log log, string formattedLog);
    }
}
