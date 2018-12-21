using System.Collections.Generic;

namespace BelleLog.Internal.Editor.Filter
{
    internal class LogFilterChain
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

        public void ResetAll()
        {
            for (int f = 0; f < _filters.Count; ++f)
            {
                _filters[f].Reset();
            }
        }

        public void RemoveAll()
        {
            _filters.Clear();
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
    }
}