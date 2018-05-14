namespace Zob.Internal
{
    public struct LogEntry
    {
        public LogLevel level;
        public string format;
        public string domain;
        public object[] args;
        public System.DateTime timestamp;
    }
}