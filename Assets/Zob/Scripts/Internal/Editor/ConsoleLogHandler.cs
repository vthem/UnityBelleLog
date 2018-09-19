using System;
using System.Collections.Generic;
using System.Text;

namespace Zob.Internal.Editor
{
    internal class ConsoleLogHandler : ILogHandler, ILogEntryContainer
    {
        private List<LogEntry> _logEntries = new List<LogEntry>();
        private StringBuilder _stringBuilder = new StringBuilder();
        private event Action<ILogEntryContainer, LogEntry> Updated;
        private List<int> _filteredLogEntries = new List<int>();
        private LogFilterEngine _filterEngine = new LogFilterEngine();

        public void AddFilter(ILogFilter filter)
        {
            _filterEngine.AddFilter(filter);
        }

        public void RemoveFilter(ILogFilter filter)
        {
            _filterEngine.RemoveFilter(filter);
        }

        public void ApplyFilters()
        {
            _filteredLogEntries = _filterEngine.Apply(_logEntries);
        }

        #region ILogEntryContainer
        int ILogEntryContainer.Count { get { return _filteredLogEntries.Count; } }

        LogEntry ILogEntryContainer.this[int index]
        {
            get
            {
                if (index < 0 || index >= _filteredLogEntries.Count)
                {
                    throw new System.IndexOutOfRangeException("ConsoleLogHandler > requested index=" + index + " count=" + _logEntries.Count);
                }
                return _logEntries[_filteredLogEntries[index]];
            }
        }

        event Action<ILogEntryContainer, LogEntry> ILogEntryContainer.Updated
        {
            add
            {
                Updated += value;
            }

            remove
            {
                Updated -= value;
            }
        }

        void ILogEntryContainer.Lock()
        {
            throw new System.NotImplementedException();
        }

        void ILogEntryContainer.Unlock()
        {
            throw new System.NotImplementedException();
        }

        void ILogEntryContainer.Clear()
        {
            _logEntries.Clear();
            _filteredLogEntries.Clear();
            _filterEngine.ResetFiltersMatchCount();
        }
        #endregion ILogEntryContainer

        #region ILogHandler
        void ILogHandler.Enqueue(LogEntry entry)
        {
            if (entry.args != null)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.AppendFormat(entry.format, entry.args);
                entry.content = _stringBuilder.ToString();
            }
            else
            {
                entry.content = entry.format;
            }
            _logEntries.Add(entry);
            if (_filterEngine.Apply(entry) == LogFilterState.Accept)
            {
                _filteredLogEntries.Add(_logEntries.Count - 1);
            }

            if (Updated != null)
            {
                Updated.Invoke(this, entry);
            }
        }
        #endregion ILogHandler
    }
}