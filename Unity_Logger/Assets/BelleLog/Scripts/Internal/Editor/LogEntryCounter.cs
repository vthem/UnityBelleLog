using BelleLog.Internal.Editor;

namespace BelleLog.Internal
{
    internal class LogEntryCounter : ITableLineCount
    {
        private ILogEntryContainer _container;

        public LogEntryCounter(ILogEntryContainer container)
        {
            _container = container;
        }

        public int Count
        {
            get
            {
                return _container.Count;
            }
        }
    }
}