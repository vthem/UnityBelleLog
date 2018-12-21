
using System;

namespace BelleLog.Internal
{
    public interface ILogFilterEnableChangedEvent
    {
        event Action FilterEnableChanged;
    }

    public interface ILogFilter
    {
        bool Enable { get; set; }
        void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination);
        void Reset();
    }
}