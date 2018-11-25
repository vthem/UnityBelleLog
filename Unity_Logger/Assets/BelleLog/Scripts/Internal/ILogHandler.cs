namespace BelleLog.Internal
{
    public interface ILogHandler
    {
        void Enqueue(LogEntry entry);
    }
}