using System;
using System.Collections.Generic;

namespace Zob.Internal
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
            _falseAction = LogFilterAction.Hold;
            Enable = true;
        }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            if (_predicate(logEntry))
            {
                action = _trueAction == LogFilterAction.Hold ? action : _trueAction;
                termination = _trueTermination;
            }
            else
            {
                action = _falseAction == LogFilterAction.Hold ? action : _falseAction;
                termination = _falseTermination;
            }
        }
    }

    internal class LogFilterEngine
    {
        private List<ILogFilter> _filters = new List<ILogFilter>();

        public void AddFilter(ILogFilter filter)
        {
            if (!_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }

        public void RemoveFilter(ILogFilter filter)
        {
            _filters.Remove(filter);
        }

        public LogFilterAction Apply(LogEntry entry)
        {
            LogFilterTermination termination;
            LogFilterAction action = LogFilterAction.Accept;
            for (int f = 0; f < _filters.Count; ++f)
            {
                if (!_filters[f].Enable)
                {
                    continue;
                }
                _filters[f].Apply(entry, ref action, out termination);
                if (termination == LogFilterTermination.Stop)
                {
                    break;
                }
            }
            return action;
        }

        public List<int> Apply(List<LogEntry> entries)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < entries.Count; ++i)
            {
                if (Apply(entries[i]) == LogFilterAction.Accept)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}