
namespace Zob
{
    public enum LogFilterAction
    {
        Continue,
        Stop
    }

    public enum LogFilterState
    {
        Accept,
        Drop,
        None
    }

    public interface ILogFilter
    {
        void Apply(LogEntry logEntry, ref LogFilterState state, out LogFilterAction action);
    }
}