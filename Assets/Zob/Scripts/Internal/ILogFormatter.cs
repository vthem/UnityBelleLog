namespace Zob.Internal
{
    public interface ILogFormatter
    {
        string Format(LogEntry entry);

        string Id { get; }
    }
}