﻿
namespace BelleLog.Internal
{
    public interface ILogFilter
    {
        bool Enable { get; set; }
        void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination);
        void Reset();
    }
}