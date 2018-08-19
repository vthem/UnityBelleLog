using System.Text;

namespace Zob
{
    public sealed class Logger
    {
        private string _domain;
        private StringBuilder _stringBuilder = new StringBuilder();

        public Logger(string domain)
        {
            _domain = domain;
        }

        public Logger()
        {
            _domain = string.Empty;
        }

        public void Trace()
        {
            var stackTrace = new System.Diagnostics.StackTrace(1, true);
            if (stackTrace.FrameCount == 0)
            {
                return;
            }

            var method = stackTrace.GetFrame(0).GetMethod();
            _stringBuilder.Length = 0;
            _stringBuilder.Append(method.ReflectedType);
            _stringBuilder.Append("::");
            _stringBuilder.Append(method.Name);

            DoLog(Internal.LogLevel.Trace, _stringBuilder.ToString(), null, stackTrace);
        }

        public void Debug(string format, params object[] args)
        {
            DoLog(Internal.LogLevel.Debug, format, args);
        }

        public void Info(string format, params object[] args)
        {
            DoLog(Internal.LogLevel.Info, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            DoLog(Internal.LogLevel.Warning, format, args);
        }

        public void Error(string format, params object[] args)
        {
            DoLog(Internal.LogLevel.Error, format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            DoLog(Internal.LogLevel.Fatal, format, args);
        }

        private void DoLog(Internal.LogLevel level, string format, params object[] args)
        {
            var stackTrace = new System.Diagnostics.StackTrace(2, true);
            if (stackTrace.FrameCount == 0)
            {
                return;
            }

            DoLog(level, format, stackTrace, args);
        }

        private void DoLog(Internal.LogLevel level, string format, System.Diagnostics.StackTrace stackTrace, params object[] args)
        {
            Internal.LogEntry entry;
            entry.args = args;
            entry.format = format;
            entry.level = Internal.LogLevel.Debug;
            entry.domain = _domain;
            entry.timestamp = System.DateTime.Now;
            entry.stackTrace = stackTrace;

            Internal.LogSystem.Log(entry);
        }
    }
}