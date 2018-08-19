namespace Zob.Internal
{
    internal interface ILogFormatter
    {
        string Format(LogEntry entry);

        string Id { get; }
    }
}