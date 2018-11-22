using Zob.Internal.Editor;

namespace Zob.Internal
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