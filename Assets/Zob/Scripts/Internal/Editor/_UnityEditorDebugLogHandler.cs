using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal static class _UnityEditorDebugLogHandler
    {
        private static System.TimeSpan defaultDuration = new System.TimeSpan(0);
        private static object[] defaultArgs = new object[0];
        private static string[] splitStackTrace = new string[] { "\n" };
        private static LogEntryStackFrame invalidFrame = new LogEntryStackFrame();

        [InitializeOnLoadMethod]
        static void _InitializeOnLoad()
        {
            Application.logMessageReceived += LogMessageReceivedHandler;
        }

        private static void LogMessageReceivedHandler(string condition, string stackTrace, LogType type)
        {

            if (stackTrace.Contains("Zob.Internal"))
            {
                return;
            }
            LogEntry entry;
            entry.args = defaultArgs;
            entry.content = string.Empty;
            entry.domain = "UnityEngine";
            entry.duration = defaultDuration;
            entry.format = condition;
            entry.frame = 0;
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    entry.level = LogLevel.Error;
                    break;
                case LogType.Warning:
                    entry.level = LogLevel.Warning;
                    break;
                case LogType.Log:
                    entry.level = LogLevel.Debug;
                    break;
                default:
                    entry.level = LogLevel.Debug;
                    break;
            }
            entry.stackTrace = ConvertStackTrace(stackTrace);
            entry.time = System.DateTime.Now;

            LogSystem.Log(entry);
        }

        //Assets/Zob/Scripts/Internal/Editor/_UnityEditorDebugLogHandler.cs(33,37): error CS1061: Type `string' does not contain a definition for `GetFrames' and no extension method `GetFrames' of type `string' could be found. Are you missing an assembly reference?

        //UnityEngine.Debug:Log(Object)
        //RuntimeLogGenerator:Awake() Assets/RuntimeLogGenerator.cs:12)

        private static LogEntryStackFrame[] ConvertStackTrace(string stackTrace)
        {
            string[] strFrames = stackTrace.Split(splitStackTrace, System.StringSplitOptions.RemoveEmptyEntries);
            int length = strFrames.Length;
            if (length == 0)
            {
                return new LogEntryStackFrame[0];
            }
            int start = 0;
            if (strFrames[0] == "UnityEngine.Debug:Log(Object)")
            {
                start = 1;
                length -= 1;
            }
            LogEntryStackFrame[] result = new LogEntryStackFrame[length];
            if (result.Length == 0)
            {
                return result;
            }
            for (int i = start; i < length; ++i)
            {
                var endClassMethodTokenPos = strFrames[i].IndexOf(')');
                if (endClassMethodTokenPos == -1)
                {
                    result[i] = invalidFrame;
                    continue;
                }
                LogEntryStackFrame internalFrame;
                var classMethodSeparatorPos = strFrames[i].IndexOf(':');
                if (classMethodSeparatorPos == -1 || classMethodSeparatorPos >= endClassMethodTokenPos)
                {
                    result[i] = invalidFrame;
                    continue;
                }
                internalFrame.className = strFrames[i].Substring(0, classMethodSeparatorPos);
                internalFrame.methodName = strFrames[i].Substring(classMethodSeparatorPos+1, endClassMethodTokenPos);
                internalFrame.
            }

            //for (int i = 0; i < frames.Length; ++i)
            //{
            //    var frame = frames[i];
            //    LogEntryStackFrame internalFrame;
            //    internalFrame.className = frame.GetMethod().ReflectedType.ToString();
            //    internalFrame.methodName = frame.GetMethod().Name;
            //    internalFrame.fileName = frame.GetFileName();
            //    internalFrame.line = frame.GetFileLineNumber();
            //    result[i] = internalFrame;
            //}
            return result;
        }
    }
}