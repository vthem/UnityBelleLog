using System.Collections.Generic;

namespace BelleLog.Internal.Editor.Filter
{
    internal class LogFilterIndexer
    {
        private List<int> _filteredLogEntries = new List<int>();

        public int Count { get { return _filteredLogEntries.Count; } }

        public void Clear()
        {
            _filteredLogEntries.Clear();
        }

        public int this[int index]
        {
            get
            {
                return _filteredLogEntries[index];
            }
        }

        public void AddEntry(LogFilterChain filters, LogEntry entry, int index)
        {
            if (filters.Apply(entry) == LogFilterAction.Accept)
            {
                _filteredLogEntries.Add(index);
            }
        }

        public void Apply(LogFilterChain filters, List<LogEntry> entries)
        {
            _filteredLogEntries = new List<int>();
            for (int i = 0; i < entries.Count; ++i)
            {
                if (filters.Apply(entries[i]) == LogFilterAction.Accept)
                {
                    _filteredLogEntries.Add(i);
                }
            }
        }
    }
}