using Zob.Writer;

namespace Zob.Internal
{
    public sealed class LogSystem : LogSingleton<LogSystem>
    {
        public bool IsInitialized { get { return _initialized; } }

        private bool _initialized = false;
        private LogHandler logHandler = new LogHandler();

        public void Log(LogEntry entry)
        {
            if (!_initialized)
            {
                return;
            }
            logHandler.Enqueue(entry);
        }

        public void Initialize(LogConfigModel config)
        {
            _initialized = true;
            ILogWriter writer = new FileLogWriter();
            writer.Open();
            logHandler.AddWriter(writer);

            ILogFormatter formatter = new DefaultLogFormatter();
            logHandler.AddFormatter(formatter);

            logHandler.Start();
        }
    }
}