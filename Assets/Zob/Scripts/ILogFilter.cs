
namespace Zob
{
    public enum LogFilterState
    {
        Continue,
        Stop
    }

    public enum LogFilterAction
    {
        Accept,
        Drop
    }

    public interface ILogFilter
    {
        void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterState state);
    }
}