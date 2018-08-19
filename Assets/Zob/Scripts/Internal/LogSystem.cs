using System.Collections.Generic;

namespace Zob.Internal
{
    internal sealed class LogSystem
    {
        private List<ILogHandler> _handlers = new List<ILogHandler>();

        public static void Log(LogEntry entry)
        {
            LogSingleton<LogSystem>.Instance.DoLog(entry);
        }

        public static void AddHandler(ILogHandler handler)
        {
            LogSingleton<LogSystem>.Instance.DoAddHandler(handler);
        }

        private void DoLog(LogEntry entry)
        {
            for (int i = 0; i < _handlers.Count; ++i)
            {
                _handlers[i].Enqueue(entry);
            }
        }

        private void DoAddHandler(ILogHandler handler)
        {
            _handlers.Add(handler);
        }
    }
}