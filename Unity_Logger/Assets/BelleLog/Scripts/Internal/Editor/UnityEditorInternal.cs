using System;
using System.Reflection;

namespace BelleLog.Internal.Editor
{
    public class UnityEditorInternal
    {
        [Flags]
        internal enum Mode
        {
            Error = 1 << 0,
            Assert = 1 << 1,
            Log = 1 << 2,
            Fatal = 1 << 4,
            DontPreprocessCondition = 1 << 5,
            AssetImportError = 1 << 6,
            AssetImportWarning = 1 << 7,
            ScriptingError = 1 << 8,
            ScriptingWarning = 1 << 9,
            ScriptingLog = 1 << 10,
            ScriptCompileError = 1 << 11,
            ScriptCompileWarning = 1 << 12,
            StickyError = 1 << 13,
            MayIgnoreLineNumber = 1 << 14,
            ReportBug = 1 << 15,
            DisplayPreviousErrorInStatusBar = 1 << 16,
            ScriptingException = 1 << 17,
            DontExtractStacktrace = 1 << 18,
            ShouldClearOnPlay = 1 << 19,
            GraphCompileError = 1 << 20,
            ScriptingAssertion = 1 << 21,
            VisualScriptingError = 1 << 22
        };

        private MethodInfo _startGettingEntriesMethod = null;
        private MethodInfo _endGettingEntriesMethod = null;
        private MethodInfo _getEntryMethod = null;
        private Type _logEntryType;
        private FieldInfo _logEntryConditionFieldInfo = null;
        private FieldInfo _logEntryModeFieldInfo = null;
        private bool _initialized = false;
        private object _logEntry;
        private readonly char[] TrimChar = new char[] { '\n', ' ' };

        public UnityEditorInternal()
        {
            try
            {
                string ns = "UnityEditorInternal";
#if UNITY_2017_1_OR_NEWER
                ns = "UnityEditor";
#endif
                var unityEditor = Assembly.Load("UnityEditor.dll");
                var logEntriesType = unityEditor.GetType(ns + ".LogEntries");
                _startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries");
                _endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries");
                _getEntryMethod = logEntriesType.GetMethod("GetEntryInternal");
                _logEntryType = unityEditor.GetType(ns + ".LogEntry");
                _logEntryConditionFieldInfo = _logEntryType.GetField("condition");
                _logEntryModeFieldInfo = _logEntryType.GetField("mode");
                _logEntry = Activator.CreateInstance(_logEntryType);
                _initialized = true;
            }
            catch (System.Exception ex)
            {
                throw new BelleLogFatalException("Fail to initialize UnityEditorInternal", ex);
            }
        }

        public void AddInternalLog()
        {
            if (!_initialized)
            {
                return;
            }
            int count = StartGettingEntries();
            for (int i = 0; i < count; ++i)
            {
                AddLogEntry(i);
            }
            EndGettingEntries();
        }

        private int StartGettingEntries()
        {

            return (int)_startGettingEntriesMethod.Invoke(null, null);
        }

        private void EndGettingEntries()
        {

            _endGettingEntriesMethod.Invoke(null, null);
        }

        public void AddLogEntry(int index)
        {
            var success = (bool)_getEntryMethod.Invoke(null, new object[] { index, _logEntry });

            if (success)
            {
                string condition = (string)_logEntryConditionFieldInfo.GetValue(_logEntry);
                string stackTrace = string.Empty;
                int crPos = condition.IndexOf('\n');
                if (crPos > 0)
                {
                    stackTrace = condition.Substring(crPos).Trim(TrimChar);
                    condition = condition.Substring(0, crPos).Trim(TrimChar);
                }
                int mode = (int)_logEntryModeFieldInfo.GetValue(_logEntry);

                UnityEditorDebugLogHandler.AddUnityInternalLog(condition, stackTrace, LogTypeFromMode(mode));
            }
        }

        private static bool HasMode(int mode, Mode modeToCheck) { return (mode & (int)modeToCheck) != 0; }

        static internal UnityEngine.LogType LogTypeFromMode(int mode)
        {
            // Errors
            if (HasMode(mode, Mode.Fatal | Mode.Assert |
                    Mode.Error | Mode.ScriptingError |
                    Mode.AssetImportError | Mode.ScriptCompileError |
                    Mode.GraphCompileError | Mode.ScriptingAssertion))
            {
                return UnityEngine.LogType.Error;
            }

            // Warnings
            if (HasMode(mode, Mode.ScriptCompileWarning | Mode.ScriptingWarning | Mode.AssetImportWarning))
            {
                return UnityEngine.LogType.Warning;
            }

            return UnityEngine.LogType.Log;
        }
    }
}