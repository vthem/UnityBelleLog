using System;
using System.Collections.Generic;
using System.Linq;

namespace Zob.Internal
{
    public enum FilterAction
    {
        Accept,
        Drop
    }

    public enum FilterState
    {
        Continue,
        Stop
    }

    class PredicateLogFilter : ILogFilter
    {
        private Func<LogEntry, bool> _predicate;
        private FilterState _stateWhenMatching;


        internal PredicateLogFilter(FilterState stateWhenMatching, Func<LogEntry, bool>  predicate)
        {
            _predicate = predicate;
            _stateWhenMatching = stateWhenMatching;
        }

        public void Apply(LogEntry logEntry, ref FilterAction action, out FilterState state)
        {
            state = FilterState.Continue;
            if (_predicate(logEntry))
            {
                state = _stateWhenMatching;
                action = FilterAction.Drop;
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

            FilterState state = FilterState.Continue;
            FilterAction action = FilterAction.Accept;
            for (int i = 0; i < entries.Count; ++i)
            {
                for (int f = 0; f < filters.Length && state == FilterState.Continue; ++f)
                {
                    filters[f].Apply(entries[i], ref action, out state);
                }
                if (action == FilterAction.Accept)
                {
                    result.Add(entries[i]);
                }
            }

            return result;
        }
    }
}