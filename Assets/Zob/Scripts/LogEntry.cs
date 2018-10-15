namespace Zob
{
    public struct LogEntry
    {
        public LogLevel level;
        public string format;
        public string domain;
        public object[] args;
        public System.Diagnostics.StackTrace stackTrace;
        public System.DateTime time;
        public System.TimeSpan duration;
        public string content;
    }
}