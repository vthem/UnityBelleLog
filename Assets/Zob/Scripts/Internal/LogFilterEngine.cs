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

        internal PredicateLogFilter(Func<LogEntry, bool> predicate, LogFilterAction onTrueAction, LogFilterAction onFalseAction, LogFilterState onTrueState)
        {
            _predicate = predicate;
            _onTrueAction = onTrueAction;
            _onFalseAction = onFalseAction;
            _onTrueState = onTrueState;
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
        private HashSet<ILogFilter> _filters = new HashSet<ILogFilter>();

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

        public List<int> Apply(List<LogEntry> entries)
        {
            List<int> result = new List<int>();
            ILogFilter[] filters = _filters.ToArray();

            for (int i = 0; i < entries.Count; ++i)
            {
                LogFilterState state = LogFilterState.None;
                for (int f = 0; f < filters.Length; ++f)
                {
                    LogFilterAction action;
                    filters[f].Apply(entries[i], ref state, out action);
                    if (action == LogFilterAction.Stop)
                    {
                        break;
                    }
                }
                if (state == LogFilterState.Accept)
                {
                    result.Add(i);
                }
            }

            return result;
        }
    }
}