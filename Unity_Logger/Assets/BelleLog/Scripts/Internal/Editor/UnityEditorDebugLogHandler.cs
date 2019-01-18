#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal static class UnityEditorDebugLogHandler
    {
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
            entry.content = condition;
            entry.domain = "UnityEngine";
            if (EditorApplication.isPlaying)
            {
                entry.duration = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
                entry.frame = Time.frameCount;
            }
            else
            {
                entry.duration = TimeSpan.FromSeconds(0.0);
                entry.frame = 0;
            }
            entry.format = null;
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

        private static LogEntryStackFrame[] ConvertStackTrace(string stackTrace)
        {
            string[] strFrames = stackTrace.Split(splitStackTrace, System.StringSplitOptions.RemoveEmptyEntries);

            List<LogEntryStackFrame> result = new List<LogEntryStackFrame>();
            for (int i = 0; i < strFrames.Length; ++i)
            {
                // UnityEngine.Debug:LogError(Object)
                // UnityEditor.DockArea:OnGUI()
                Match match = Regex.Match(strFrames[i], @"(?<class>^[^:]*):(?<method>[^(]*)\(([^)]*)\)( \(at (?<file>[^:]*):(?<line>[0-9]*)\)$)?");
                if (match.Success)
                {
                    LogEntryStackFrame stackFrame;
                    stackFrame.className = match.Groups["class"].Value;
                    stackFrame.methodName = match.Groups["method"].Value;
                    stackFrame.fileName = new Uri(projectPath + "/" + match.Groups["file"].Value).LocalPath;
                    stackFrame.line = 0;
                    var line = match.Groups["line"].Value;
                    if (!string.IsNullOrEmpty(line))
                    {
                        stackFrame.line = Convert.ToInt32(line);
                    }
                    result.Add(stackFrame);
                }
                // System.Reflection.MonoMethod.Invoke (System.Object obj, BindingFlags invokeAttr, System.Reflection.Binder binder, System.Object[] parameters, System.Globalization.CultureInfo culture) (at /Users/builduser/buildslave/mono/build/mcs/class/corlib/System.Reflection/MonoMethod.cs:222)
                // UnityEngine.GUILayoutGroup.GetNext () (at C:/buildslave/unity/build/Runtime/IMGUI/Managed/LayoutGroup.cs:115)
                match = Regex.Match(strFrames[i], @"(?<func>^[^(]*) \(([^)]*)\) \(at (?<file>([a-zA-Z]:)?[^:]*):(?<line>[0-9]*)\)$");
                if (match.Success)
                {
                    LogEntryStackFrame stackFrame;
                    var funcGroup = match.Groups["func"].Value;
                    int dotPos = funcGroup.LastIndexOf('.');
                    if (dotPos > 0 && dotPos < funcGroup.Length - 1)
                    {
                        stackFrame.className = funcGroup.Substring(0, dotPos);
                        stackFrame.methodName = funcGroup.Substring(dotPos + 1);
                    }
                    else
                    {
                        stackFrame.className = "unknown";
                        stackFrame.methodName = "unknown";
                    }

                    stackFrame.fileName = new Uri(projectPath + "/" + match.Groups["file"].Value).LocalPath;
                    stackFrame.line = 0;
                    var line = match.Groups["line"].Value;
                    if (!string.IsNullOrEmpty(line))
                    {
                        stackFrame.line = Convert.ToInt32(line);
                    }
                    result.Add(stackFrame);
                }
            }
            return result.ToArray();
        }
    }
}
#endif