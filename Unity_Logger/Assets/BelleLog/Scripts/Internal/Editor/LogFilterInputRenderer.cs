using BelleLog.Internal.Editor.Filter;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogFilterInputRenderer : ILogFilter
    {
        private string _filterInputText;
        private ConsoleLogHandler _logHandler;
        private LogFilterChain _filterChain = new LogFilterChain();

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
            //GUI.SetNextControlName("foobar");
            _filterInputText = EditorGUILayout.TextArea(_filterInputText);
            //if (GUI.GetNameOfFocusedControl() == "foobar")
            //{
            //    if (Event.current.control && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            //    {
            //        Debug.Log("parse it!");
            //        ParseInputText();
            //    }
            //}
            if (GUILayout.Button("Apply"))
            {
                ParseInputText();
            }

        }

        public void ParseInputText()
        {
            var matches = Regex.Matches(_filterInputText, @"(?<cmd>-[xi]):?(?<opt>[a-z]*) (('(?<v1>(.*))')|(?<v2>([^ ]*)))");


            Debug.Log("count=" + matches.Count);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Debug.Log("cmd=" + groups["cmd"] + " opt=" + groups["opt"] + " v1=" + groups["v1"] + " v2=" + groups["v2"]);
            }
        }

        public void Reset()
        {
        }
    }
}