using System;
using System.Reflection;

namespace Zob.Internal.Editor
{
    public class UnityEditorInternal
    {
        private MethodInfo _startGettingEntriesMethod = null;
        private MethodInfo _endGettingEntriesMethod = null;
        private MethodInfo _getEntryMethod = null;
        private Type _logEntryType;
        private FieldInfo _logEntryConditionFieldInfo = null;
        private FieldInfo _logEntryErrorNumFieldInfo = null;
        private FieldInfo _logEntryFileFieldInfo = null;
        private FieldInfo _logEntryLineFieldInfo = null;
        private bool _initialized = false;
        private object _logEntry;
        private readonly char[] TrimChar = new char[] { '\n', ' ' };

        public UnityEditorInternal()
        {
            try
            {
                var unityEditor = Assembly.Load("UnityEditor.dll");
                var logEntriesType = unityEditor.GetType("UnityEditorInternal.LogEntries");
                _startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries");
                _endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries");
                _getEntryMethod = logEntriesType.GetMethod("GetEntryInternal");
                _logEntryType = unityEditor.GetType("UnityEditorInternal.LogEntry");
                _logEntryConditionFieldInfo = _logEntryType.GetField("condition");
                _logEntryErrorNumFieldInfo = _logEntryType.GetField("errorNum");
                _logEntryFileFieldInfo = _logEntryType.GetField("file");
                _logEntryLineFieldInfo = _logEntryType.GetField("line");
                _logEntry = Activator.CreateInstance(_logEntryType);
                _initialized = true;
            }
            catch (System.Exception ex)
            {
                throw new ZobFatalException("Fail to initialize UnityEditorInternal", ex);
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

            string condition = (string)_logEntryConditionFieldInfo.GetValue(_logEntry);
            string stackTrace = string.Empty;
            int crPos = condition.IndexOf('\n');
            if (crPos > 0)
            {
                stackTrace = condition.Substring(crPos).Trim(TrimChar);
                condition = condition.Substring(0, crPos).Trim(TrimChar);
            }
            //string file = (string)_logEntryFileFieldInfo.GetValue(_logEntry);
            //int line = (int)_logEntryLineFieldInfo.GetValue(_logEntry);
            int errorNum = (int)_logEntryErrorNumFieldInfo.GetValue(_logEntry);

            _UnityEditorDebugLogHandler.AddUnityInternalLog(condition, stackTrace, (UnityEngine.LogType)errorNum);
        }
    }
}