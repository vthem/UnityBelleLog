namespace Zob.Internal.Editor
{
    internal interface ILogEntryContainer
    {
        void Lock();
        void Unlock();
        int Count { get; }
        LogEntry this[int index] { get; }
        string Content(int index);
    }
}