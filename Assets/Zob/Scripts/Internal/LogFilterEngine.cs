using System;
using System.Collections.Generic;

namespace Zob.Internal
{
    internal class CollapseLogFilter : ILogFilter
    {
        private List<int> _collapseCount = new List<int>();
        private string _lastContent = string.Empty;

        public bool Enable { get; set; }

        public int CollapseCount(int index)
        {
            return 0;
        }

        public void Reset()
        {
            _collapseCount.Clear();
            _lastContent = string.Empty;
        }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            if (!string.IsNullOrEmpty(_lastContent) && logEntry.content == _lastContent)
            {
                _collapseCount[_collapseCount.Count - 1] = _collapseCount[_collapseCount.Count - 1] + 1;
                action = LogFilterAction.Reject;
                termination = LogFilterTermination.Stop;
            }
            else
            {
                action = LogFilterAction.Accept;
                termination = LogFilterTermination.Continue;
                _collapseCount.Add(0);
            }
        }
    }

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
            for (int f = 0; f < _filters.Count; ++f)
            {
                _filters[f].Reset();
            }

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