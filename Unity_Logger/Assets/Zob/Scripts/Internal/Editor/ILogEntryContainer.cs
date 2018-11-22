using System;

namespace Zob.Internal.Editor
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