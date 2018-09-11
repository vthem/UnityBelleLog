using System;

namespace Zob.Internal.Editor
{
    internal interface ILogEntryContainer
    {
        void Lock();
        void Unlock();
        int Count { get; }
        LogEntry this[int index] { get; }
        string Content(int index);
        void Clear();
        event Action<ILogEntryContainer, LogEntry> Updated;

        void AddFilter(ILogFilter filter);
        void Removeilter(ILogFilter filter);
    }
}