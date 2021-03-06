﻿using System.Text;

#if UNITY_EDITOR
namespace BelleLog.Internal
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

            DoLog(LogLevel.Trace, _stringBuilder.ToString(), stackTrace, null);
        }

        public void Debug(string format, params object[] args)
        {
            DoLog(LogLevel.Debug, format, args);
        }

        public void Info(string format, params object[] args)
        {
            DoLog(LogLevel.Info, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            DoLog(LogLevel.Warning, format, args);
        }

        public void Error(string format, params object[] args)
        {
            DoLog(LogLevel.Error, format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            DoLog(LogLevel.Fatal, format, args);
        }

        private void DoLog(LogLevel level, string format, params object[] args)
        {
            var stackTrace = new System.Diagnostics.StackTrace(2, true);
            if (stackTrace.FrameCount == 0)
            {
                return;
            }

            DoLog(level, format, stackTrace, args);
        }

        private void DoLog(LogLevel level, string format, System.Diagnostics.StackTrace stackTrace, params object[] args)
        {
            LogEntry entry;
            entry.args = args;
            entry.format = format;
            entry.level = level;
            entry.domain = _domain;
            entry.time = System.DateTime.Now;
            entry.duration = System.TimeSpan.FromSeconds(UnityEngine.Time.realtimeSinceStartup);
            entry.stackTrace = ConvertStackTrace(stackTrace);
            entry.content = string.Empty;
            entry.frame = UnityEngine.Time.frameCount;

            Internal.LogSystem.Log(entry);
        }

        private static LogEntryStackFrame[] ConvertStackTrace(System.Diagnostics.StackTrace stackTrace)
        {
            var frames = stackTrace.GetFrames();
            LogEntryStackFrame[] result = new LogEntryStackFrame[frames.Length];
            if (result.Length == 0)
            {
                return result;
            }

            for (int i = 0; i < frames.Length; ++i)
            {
                var frame = frames[i];
                LogEntryStackFrame internalFrame;
                internalFrame.className = frame.GetMethod().ReflectedType.ToString();
                internalFrame.methodName = frame.GetMethod().Name;
                internalFrame.fileName = frame.GetFileName();
                internalFrame.line = frame.GetFileLineNumber();
                result[i] = internalFrame;
            }
            return result;
        }
    }
}
#endif