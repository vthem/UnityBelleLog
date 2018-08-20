using System.Collections.Generic;
using System.Text;

namespace Zob.Internal.Editor
{
    internal class ConsoleLogHandler : ILogHandler, ILogEntryContainer
    {
        private List<LogEntry> _logEntries = new List<LogEntry>();
        private List<string> _contents = new List<string>();
        private StringBuilder _stringBuilder = new StringBuilder();

        int ILogEntryContainer.Count { get { return _logEntries.Count; } }

        LogEntry ILogEntryContainer.this[int index]
        {
            get
            {
                if (index < 0 || index >= _logEntries.Count)
                {
                    throw new System.IndexOutOfRangeException("ConsoleLogHandler > requested index=" + index + " count=" + _logEntries.Count);
                }
                return _logEntries[index];
            }
        }

        event System.Action<ILogEntryContainer, LogEntry> ILogEntryContainer.Updated;


        void ILogEntryContainer.Lock()
        {
            throw new System.NotImplementedException();
        }

        void ILogEntryContainer.Unlock()
        {
            throw new System.NotImplementedException();
        }

        void ILogHandler.Enqueue(LogEntry entry)
        {
            _logEntries.Add(entry);
            _stringBuilder.Length = 0;
            _stringBuilder.AppendFormat(entry.format, entry.args);
            _contents.Add(_stringBuilder.ToString());
        }

        string ILogEntryContainer.Content(int index)
        {
            if (index < 0 || index >= _contents.Count)
            {
                throw new System.IndexOutOfRangeException("ConsoleLogHandler > requested index=" + index + " count=" + _contents.Count);
            }
            return _contents[index];
        }

        void ILogEntryContainer.Clear()
        {
            _logEntries.Clear();
            _contents.Clear();
        }
    }
}