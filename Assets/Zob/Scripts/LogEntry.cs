namespace Zob
{
    public struct LogEntryStackFrame
    {
        public string className;
        public string methodName;
        public string fileName;
        public int line;
    }

    public struct LogEntry
    {
        public LogLevel level;
        public string format;
        public string domain;
        public object[] args;
        public LogEntryStackFrame[] stackTrace;
        public long frame;
        public System.DateTime time;
        public System.TimeSpan duration;
        public string content;
    }
}