using System;
using System.Reflection;

namespace Zob.Internal.Editor
{
    public class UnityEditorInternal
    {
        private MethodInfo _startGettingEntriesMethod = null;
        private MethodInfo _endGettingEntriesMethod = null;
        private MethodInfo _getEntryMethod = null;
        private FieldInfo _logEntryConditionFieldInfo = null;
        private FieldInfo _logEntryErrorNumFieldInfo = null;
        private FieldInfo _logEntryFileFieldInfo = null;
        private FieldInfo _logEntryLineFieldInfo = null;
        private bool _initialized = false;


        public UnityEditorInternal()
        {
            try
            {
                var unityEditor = Assembly.Load("UnityEditor.dll");
                var logEntriesType = unityEditor.GetType("UnityEditorInternal.LogEntries");
                _startGettingEntriesMethod = logEntriesType.GetMethod("StartGettingEntries");
                _endGettingEntriesMethod = logEntriesType.GetMethod("EndGettingEntries");
                _getEntryMethod = logEntriesType.GetMethod("GetEntryInternal");
                var logEntryType = unityEditor.GetType("UnityEditorInternal.LogEntry");
                _logEntryConditionFieldInfo = logEntriesType.GetField("condition");
                _logEntryErrorNumFieldInfo = logEntriesType.GetField("errorNum");
                _logEntryFileFieldInfo = logEntriesType.GetField("file");
                _logEntryLineFieldInfo = logEntriesType.GetField("line");
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
                LogSystem.Log(GetLogEntry(i));
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

        public LogEntry GetLogEntry(int index)
        {


            object unityEntry = new object();

            _getEntryMethod.Invoke(null, new object[] { index, unityEntry });
            string condition = (string)_logEntryConditionFieldInfo.GetValue(unityEntry);
            string file = (string)_logEntryFileFieldInfo.GetValue(unityEntry);
            int errorNum = (int)_logEntryErrorNumFieldInfo.GetValue(unityEntry);
            int line = (int)_logEntryLineFieldInfo.GetValue(unityEntry);

            LogEntry entry;
            entry.args = null;
            entry.content = string.Empty;
            entry.domain = "UnityEngine";
            entry.duration = new TimeSpan(0);
            entry.format = "tada!";
            entry.frame = 0;
            entry.level = LogLevel.Error;
            entry.time = System.DateTime.Now;
            entry.stackTrace = new LogEntryStackFrame[0];
            return entry;
        }
    }
}