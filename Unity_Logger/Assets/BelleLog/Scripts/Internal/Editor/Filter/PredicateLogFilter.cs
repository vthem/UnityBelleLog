
using System;

namespace BelleLog.Internal.Editor.Filter
{
    internal class PredicateLogFilter : ILogFilter
    {
        private Func<LogEntry, bool> _predicate;
        private LogFilterTermination _trueTermination;
        private LogFilterTermination _falseTermination;
        private LogFilterAction _trueAction;
        private LogFilterAction _falseAction;

        public bool Enable { get; set; }

        internal PredicateLogFilter(Func<LogEntry, bool> predicate, LogFilterTermination trueTermination, LogFilterTermination falseTermination, LogFilterAction trueAction, LogFilterAction falseAction)
        {
            _predicate = predicate;
            _trueTermination = trueTermination;
            _falseTermination = falseTermination;
            _trueAction = trueAction;
            _falseAction = falseAction;
            Enable = true;
        }

        internal PredicateLogFilter(Func<LogEntry, bool> predicate)
        {
            _predicate = predicate;
            _trueTermination = LogFilterTermination.Stop;
            _falseTermination = LogFilterTermination.Continue;
            _trueAction = LogFilterAction.Reject;
            _falseAction = LogFilterAction.Keep;
            Enable = true;
        }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            if (_predicate(logEntry))
            {
                action = _trueAction == LogFilterAction.Keep ? action : _trueAction;
                termination = _trueTermination;
            }
            else
            {
                action = _falseAction == LogFilterAction.Keep ? action : _falseAction;
                termination = _falseTermination;
            }
        }

        public void Reset()
        {

        }
    }
}