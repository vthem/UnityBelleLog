using System;
using System.Collections.Generic;
using System.Linq;

namespace Zob.Internal
{


    class PredicateLogFilter : ILogFilter
    {
        private Func<LogEntry, bool> _predicate;
        private LogFilterState _stateWhenMatching;


        internal PredicateLogFilter(LogFilterState stateWhenMatching, Func<LogEntry, bool>  predicate)
        {
            _predicate = predicate;
            _stateWhenMatching = stateWhenMatching;
        }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterState state)
        {
            state = LogFilterState.Continue;
            if (_predicate(logEntry))
            {
                state = _stateWhenMatching;
                action = LogFilterAction.Drop;
                return;
            }
        }
    }

    class LogFilterEngine
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

        public List<LogEntry> Apply(List<LogEntry> entries)
        {
            List<LogEntry> result = new List<LogEntry>();
            ILogFilter[] filters = _filters.ToArray();

            LogFilterState state = LogFilterState.Continue;
            LogFilterAction action = LogFilterAction.Accept;
            for (int i = 0; i < entries.Count; ++i)
            {
                for (int f = 0; f < filters.Length && state == LogFilterState.Continue; ++f)
                {
                    filters[f].Apply(entries[i], ref action, out state);
                }
                if (action == LogFilterAction.Accept)
                {
                    result.Add(entries[i]);
                }
            }

            return result;
        }
    }
}