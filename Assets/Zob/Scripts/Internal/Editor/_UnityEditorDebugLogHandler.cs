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
        private static string projectPath = string.Empty;

        [InitializeOnLoadMethod]
        static void _InitializeOnLoad()
        {
            projectPath = Application.dataPath.Replace("/Assets", "");
            Application.logMessageReceived += LogMessageReceivedHandler;
        }

        public static void AddUnityInternalLog(string condition, string stacktrace, LogType type)
        {
            LogMessageReceivedHandler(condition, stacktrace, type);
        }

        private static void LogMessageReceivedHandler(string condition, string stackTrace, LogType type)
        {

            if (stackTrace.Contains("_UnityEditorDebugLogHandler.LogMessageReceivedHandler"))
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

            Regex rx = new Regex(@"^(?<file>[A-Za-z\/_@~0-9]*.cs)\((?<line>[0-9]*),[0-9]*\)");
            Match match = rx.Match(condition);
            if (match.Success)
            {
                entry.stackTrace = new LogEntryStackFrame[1];
                entry.stackTrace[0].fileName = new Uri(projectPath + "/" + match.Groups["file"].Value).LocalPath;
                var line = match.Groups["line"].Value;
                if (!string.IsNullOrEmpty(line))
                {
                    entry.stackTrace[0].line = Convert.ToInt32(line);
                }
                entry.stackTrace[0].methodName = string.Empty;
                entry.stackTrace[0].className = string.Empty;
            }
            else
            {
                entry.stackTrace = ConvertStackTrace(stackTrace);
            }
            entry.time = System.DateTime.Now;

            LogSystem.Log(entry);
        }

        /*
Example of stacktrace:
=======================

UnityEngine.Debug:LogError(Object)

UnityEditor.DockArea:OnGUI()

System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at /Users/builduser/buildslave/mono/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:222)

UnityEngine.Debug:LogError(Object)

Zob.Internal.Editor.DebugConsole:OnGUI() (at Assets/Editor/DebugConsole.cs:116)

UnityEditor.DockArea:OnGUI()
        */
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
            for (int i = start; i < strFrames.Length; ++i)
            {
                Regex rx = new Regex(@"(((?<call>.*) \(\))|((?<class>^[a-zA-Z_\.0-9]*):(?<func>[.a-zA-Z_0-9]*)))(.*at (?<file>[a-zA-Z\/]*.cs):(?<line>[0-9]*))?");
                Match match = rx.Match(strFrames[i]);
                result[i - start].className = match.Groups["class"].Value;
                result[i - start].methodName = match.Groups["func"].Value;
                result[i - start].fileName = new Uri(projectPath + "/" + match.Groups["file"].Value).LocalPath;
                result[i - start].line = 0;
                var line = match.Groups["line"].Value;
                if (!string.IsNullOrEmpty(line))
                {
                    result[i - start].line = Convert.ToInt32(line);
                }
            }
            return result;
        }
    }
}