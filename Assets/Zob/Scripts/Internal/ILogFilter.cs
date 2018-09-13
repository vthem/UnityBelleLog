
namespace Zob.Internal
{
    public interface ILogFilter
    {
        void Apply(LogEntry logEntry, ref FilterAction action, out FilterState state);
    }
}