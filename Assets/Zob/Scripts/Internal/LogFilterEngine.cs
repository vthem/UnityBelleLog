using System;
using System.Collections.Generic;
using System.Linq;

namespace Zob.Internal
{
    internal class PredicateLogFilter : ILogFilter
    {
        private Func<LogEntry, bool> _predicate;
        private LogFilterAction _onTrueAction;
        private LogFilterAction _onFalseAction;
        private LogFilterState _onTrueState;

        public bool Enable { get; set; }

        internal PredicateLogFilter(Func<LogEntry, bool> predicate, LogFilterAction onTrueAction, LogFilterAction onFalseAction, LogFilterState onTrueState)
        {
            _predicate = predicate;
            _onTrueAction = onTrueAction;
            _onFalseAction = onFalseAction;
            _onTrueState = onTrueState;
            Enable = true;
        }

        public void Apply(LogEntry logEntry, ref LogFilterState state, out LogFilterAction action)
        {
            if (_predicate(logEntry))
            {
                state = _onTrueState;
                action = _onTrueAction;
            }
            else
            {
                action = _onFalseAction;
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

        public LogFilterState Apply(LogEntry entry)
        {
            LogFilterState state = LogFilterState.None;
            for (int f = 0; f < _filters.Count; ++f)
            {
                if (!_filters[f].Enable)
                {
                    continue;
                }
                LogFilterAction action;
                _filters[f].Apply(entry, ref state, out action);
                if (action == LogFilterAction.Stop)
                {
                    break;
                }
            }
            return state;
        }

        public List<int> Apply(List<LogEntry> entries)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < entries.Count; ++i)
            {
                if (Apply(entries[i]) == LogFilterState.Accept)
                {
                    result.Add(i);
                }
            }
            return result;
        }
    }
}