using System.Collections.Generic;

namespace Zob.Internal.Editor.Filter
{
    internal class CollapseLogFilter : ILogFilter
    {
        private List<int> _collapseCount = new List<int>();
        private string _lastContent = string.Empty;

        public bool Enable { get; set; }

        public int CollapseCount(int index)
        {
            if (index >= 0 && index < _collapseCount.Count)
            {
                return _collapseCount[index];
            }
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
            _lastContent = logEntry.content;
        }
    }
}