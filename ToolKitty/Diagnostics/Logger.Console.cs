using System;

namespace ToolKitty.Diagnostics
{
    public class ConsoleLogger : OutputLogger
    {
        protected override void OnLog(Log log, string formattedLog)
        {
            Console.ForegroundColor = GetForeground(log.Level);
            Console.WriteLine(formattedLog);
            Console.ResetColor();
        }

        static ConsoleColor GetForeground(LogLevel level)
        {
            switch (level) {
                case LogLevel.Trace:
                    return ConsoleColor.Gray;
                case LogLevel.Debug:
                    return ConsoleColor.Green;
                case LogLevel.Default:
                    return ConsoleColor.White;
                case LogLevel.Warning:
                    return ConsoleColor.Yellow;
                case LogLevel.Critical:
                    return ConsoleColor.Magenta;
                case LogLevel.Exception:
                    return ConsoleColor.DarkRed;
                default: throw new NotSupportedException($"{level}");
            }
        }
    }
}
