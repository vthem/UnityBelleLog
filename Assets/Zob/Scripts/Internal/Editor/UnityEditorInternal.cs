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


        public UnityEditorInternal()
        {
            try
            {
                Assembly unityEditor = Assembly.Load("UnityEditor.dll");
                Type logEntries = unityEditor.GetType("UnityEditorInternal.LogEntries");
                _startGettingEntriesMethod = logEntries.GetMethod("StartGettingEntries");
            }
            catch (System.Exception _ex)
            {
                throw new ZobFatalException("Fail to initialize _startGettingEntriesMethod", _ex);
            }

            try
            {
                if (null == _endGettingEntriesMethod)
                {
                    Assembly unityEditor = Assembly.Load("UnityEditor.dll");
                    Type logEntries = unityEditor.GetType("UnityEditorInternal.LogEntries");
                    _endGettingEntriesMethod = logEntries.GetMethod("EndGettingEntries");
                }
            }
            catch (System.Exception _ex)
            {
                throw new ZobFatalException("Fail to initialize _endGettingEntriesMethod", _ex);
            }

            try
            {
                if (null == _getEntryMethod)
                {
                    Assembly unityEditor = Assembly.Load("UnityEditor.dll");
                    Type logEntries = unityEditor.GetType("UnityEditorInternal.LogEntries");
                    _startGettingEntriesMethod = logEntries.GetMethod("GetCount");
                }
            }
            catch (System.Exception _ex)
            {
                throw new ZobFatalException("Fail to initialize _getEntryMethod", _ex);
            }
        }

        public void AddInternalLog()
        {
            int count = StartGettingEntries();
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

            return (int)_startGettingEntriesMethod.Invoke(null, null);

        }
    }
}