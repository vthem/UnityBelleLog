using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace BelleLog.Internal
{
    internal sealed class LogSystem
    {
        private List<ILogHandler> _handlers = new List<ILogHandler>();

        public static void Log(LogEntry entry)
        {
            LogSingleton<LogSystem>.Instance.InnerLog(entry);
        }

        public static void AddHandler(ILogHandler handler)
        {
            LogSingleton<LogSystem>.Instance.InnerAddHandler(handler);
        }

        private void InnerLog(LogEntry entry)
        {
            for (int i = 0; i < _handlers.Count; ++i)
            {
                _handlers[i].Enqueue(entry);
            }
        }

        private void InnerAddHandler(ILogHandler handler)
        {
            _handlers.Add(handler);
        }
    }
}