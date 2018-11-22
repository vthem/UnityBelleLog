namespace Zob.Internal
{
    public interface ILogHandler
    {
        void Enqueue(LogEntry entry);
    }
}