using System.Collections.ObjectModel;

namespace ToolKitty.Diagnostics
{
    public class RelayLogger : Collection<ILogger>, ILogger
    {
        public void Log(Log log)
        {
            foreach (var logger in this) {
                logger.Log(log);
            }
        }
    }
}
