namespace Zob
{
    public sealed class Logger
    {
        private string _domain;
        private Internal.LogSystem _logSystem;

        public Logger(string domain)
        {
            _domain = domain;
        }

        public void Trace()
        {
            InitializeOnce();

            Internal.LogEntry entry;
            entry.args = null;
            entry.format = "Trace function";
            entry.level = Internal.LogLevel.Trace;
            entry.domain = _domain;
            entry.timestamp = System.DateTime.Now;
            _logSystem.Log(entry);
        }

        private void InitializeOnce()
        {
            if (null == _logSystem)
            {
                _logSystem = Internal.LogSystem.Instance;
            }
        }
    }
}