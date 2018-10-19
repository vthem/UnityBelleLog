using System;
using System.Collections.Generic;
using System.Text;

namespace Zob.Internal.Editor
{
    internal class ConsoleLogHandler : ILogHandler, ILogEntryContainer
    {
        private readonly List<LogEntry> _logEntries = new List<LogEntry>();
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private event Action<ILogEntryContainer, LogEntry> Updated;
        private List<int> _filteredLogEntries = new List<int>();
        private readonly LogFilterEngine _filterEngine = new LogFilterEngine();
        private readonly uint[] _logEntryCountByLevel = new uint[]
        {
            0, // Trace
            0, // Debug
            0, // Info
            0, // Warning,
            0, // Error,
            0 // Fatal
        };

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
                    throw new System.IndexOutOfRangeException("ConsoleLogHandler > requested index=" + index + " count=" + _filteredLogEntries.Count);
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

        void ILogEntryContainer.Clear()
        {
            _logEntries.Clear();
            _filteredLogEntries.Clear();
            for (int i = 0; i < _logEntryCountByLevel.Length; ++i)
            {
                _logEntryCountByLevel[i] = 0;
            }
        }

        public uint CountByLevel(LogLevel level)
        {
            return _logEntryCountByLevel[(int)level];
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
            if (_filterEngine.Apply(entry) == LogFilterAction.Accept)
            {
                _filteredLogEntries.Add(_logEntries.Count - 1);
            }

            _logEntryCountByLevel[(int)entry.level] += 1;

            if (Updated != null)
            {
                Updated.Invoke(this, entry);
            }
        }
        #endregion ILogHandler
    }
}