#if UNITY_EDITOR
using System;

namespace BelleLog.Internal.Editor
{
    internal interface ILogEntryContainer
    {
        int Count { get; }
        LogEntry this[int index] { get; }
        void Clear();

        event Action<ILogEntryContainer, LogEntry> Updated;

        uint CountByLevel(LogLevel level);
    }
}
#endif