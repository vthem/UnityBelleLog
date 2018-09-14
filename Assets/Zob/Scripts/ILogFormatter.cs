namespace Zob
{
    public interface ILogFormatter
    {
        string Format(LogEntry entry);

        string Id { get; }
    }
}