namespace BelleLog.Internal
{
    public interface ILogFormatter
    {
        string Format(LogEntry entry);

        string Id { get; }
    }
}