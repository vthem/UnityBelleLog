using Zob.Writer;

namespace Zob.Internal
{
    public sealed class LogSystem : LogSingleton<LogSystem>
    {
        private LogHandler logHandler = new LogHandler();

        public void Log(LogEntry entry)
        {
            logHandler.Enqueue(entry);
        }

        public LogSystem()
        {
            ILogWriter writer = new FileLogWriter("default.log");
            writer.Open();
            logHandler.AddWriter(writer);

            ILogFormatter formatter = new DefaultLogFormatter();
            logHandler.AddFormatter(formatter);

            logHandler.Start();
        }
    }
}