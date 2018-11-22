using System.Collections.Generic;

namespace Zob.Internal.Editor.Filter
{
    internal class LogFilterIndexer
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