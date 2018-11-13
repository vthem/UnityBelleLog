using System;
using System.Text.RegularExpressions;
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

            Regex rx = new Regex(@"#^(?<file>[A-Za-z\/_@~0-9]*.cs)\((?<line>[0-9]*),[0-9]*\)#");
            Match match = rx.Match(condition);
            if (Regex.Match(condition, @"/: (error)|(warning) CS[0-9]*:/").Success)
            {
                entry.stackTrace = new LogEntryStackFrame[1];
                entry.stackTrace[0].fileName = match.Groups["file"].Value;
                entry.stackTrace[0].line = Convert.ToInt32(match.Groups["line"].Value);
                entry.stackTrace[0].methodName = string.Empty;
                entry.stackTrace[0].fileName = string.Empty;
            }
            else
            {
                entry.stackTrace = ConvertStackTrace(stackTrace);
            }
            entry.time = System.DateTime.Now;

            LogSystem.Log(entry);
        }

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
                Regex rx = new Regex(@"/(?<class>^[a-zA-Z_\.0-9]*):(?<func>[a-zA-Z_0-9]*).*at (?<file>[a-zA-Z\/]*.cs):(?<line>[0-9]*)/");
                Match match = rx.Match(strFrames[i]);
                if (match.Groups.Count == 4)
                {
                    result[i].className = match.Groups["class"].Value;
                    result[i].methodName = match.Groups["func"].Value;
                    result[i].fileName = match.Groups["file"].Value;
                    result[i].line = Convert.ToInt32(match.Groups["line"].Value);
                }
                else
                {
                    result[i] = invalidFrame;
                }
            }
            return result;
        }
    }
}