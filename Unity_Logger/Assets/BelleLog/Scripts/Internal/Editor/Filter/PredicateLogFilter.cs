
using System;

namespace BelleLog.Internal.Editor.Filter
{
    internal class PredicateLogFilter : ILogFilter
    {
        public Func<LogEntry, bool> Predicate { get; set; }
        public LogFilterTermination TrueTermination { get; set; }
        public LogFilterTermination FalseTermination { get; set; }
        public LogFilterAction TrueAction { get; set; }
        public LogFilterAction FalseAction { get; set; }

        public bool Enable { get; set; }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            if (Predicate(logEntry))
            {
                action = TrueAction == LogFilterAction.Keep ? action : TrueAction;
                termination = TrueTermination;
            }
            else
            {
                action = FalseAction == LogFilterAction.Keep ? action : FalseAction;
                termination = FalseTermination;
            }
        }

        public void Reset()
        {

        }
    }
}