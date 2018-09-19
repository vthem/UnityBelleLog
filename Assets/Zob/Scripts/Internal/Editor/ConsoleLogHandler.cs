using System;
using System.Collections.Generic;
using System.Text;

namespace Zob.Internal.Editor
{
    internal class ConsoleLogHandler : ILogHandler, ILogEntryContainer
    {
        private List<LogEntry> _logEntries = new List<LogEntry>();
        private List<string> _contents = new List<string>();
        private StringBuilder _stringBuilder = new StringBuilder();
        private event Action<ILogEntryContainer, LogEntry> Updated;
        private List<int> _filteredLogEntries = new List<int>();
        private LogFilterEngine _filterEngine = new LogFilterEngine();

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

        string ILogEntryContainer.Content(int index)
        {
            if (index < 0 || index >= _filteredLogEntries.Count)
            {
                throw new System.IndexOutOfRangeException("ConsoleLogHandler > requested index=" + index + " count=" + _contents.Count);
            }
            return _contents[_filteredLogEntries[index]];
        }

        void ILogEntryContainer.Clear()
        {
            _logEntries.Clear();
            _contents.Clear();
            _filteredLogEntries.Clear();
        }
        #endregion ILogEntryContainer

        #region ILogHandler
        void ILogHandler.Enqueue(LogEntry entry)
        {
            _logEntries.Add(entry);
            if (entry.args != null)
            {
                _stringBuilder.Length = 0;
                _stringBuilder.AppendFormat(entry.format, entry.args);
                _contents.Add(_stringBuilder.ToString());
            }
            else
            {
                _contents.Add(entry.format);
            }

            if (Updated != null)
            {
                Updated.Invoke(this, entry);
            }
        }

        public void AddFilter(ILogFilter filter)
        {
            _filterEngine.AddFilter(filter);
        }

        public void Removeilter(ILogFilter filter)
        {
            _filterEngine.RemoveFilter(filter);
        }
        #endregion ILogHandler
    }
}