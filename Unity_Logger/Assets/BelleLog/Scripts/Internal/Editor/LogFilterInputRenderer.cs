using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogFilterInputRenderer : ILogFilter
    {
        private string _filterInputCommand;
        private ConsoleLogHandler _logHandler;

        public LogFilterInputRenderer(ConsoleLogHandler logHandler)
        {
            Enable = false;
        }

        public bool Enable { get; set; }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            throw new System.NotImplementedException();
        }

        public void OnGUI()
        {
            GUI.SetNextControlName("foobar");
            _filterInputCommand = EditorGUILayout.TextArea(_filterInputCommand);
            if (GUI.GetNameOfFocusedControl() == "foobar")
            {
                if (Event.current.control && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
                {
                    Debug.Log("ahahaha!!!");
                }
            }
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}