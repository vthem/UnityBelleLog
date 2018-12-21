using BelleLog.Internal.Editor.Filter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogFilterInputRenderer : ILogFilter
    {
        enum Opt
        {
            StackTrace,
            StackTraceFile,
            Domain,
            Content
        }

        private string _filterInputText;
        private ConsoleLogHandler _logHandler;

        Dictionary<string, Opt> strToOpt = new Dictionary<string, Opt>
                {
                    {"st", Opt.StackTrace },
                    {"d", Opt.Domain },
                    {"stf", Opt.StackTraceFile }
                };

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


            List<ILogFilter> filters = new List<ILogFilter>();
            Debug.Log("count=" + matches.Count);
            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                Debug.Log("cmd=" + groups["cmd"] + " opt=" + groups["opt"] + " v1=" + groups["v1"] + " v2=" + groups["v2"]);
                var opt = groups["opt"].Value;
                var v1 = groups["v1"].Value;
                var v2 = groups["v2"].Value;
                var v = v1;
                if (string.IsNullOrEmpty(v1))
                {
                    v = v2;
                }

                // opt list:
                // no opt: log content
                // st: stacktrace
                // d: domain
                // l: level
                // stf: stacktrace file

                Func<LogEntry, bool> predicate;
                bool exclude = groups["cmd"].Value == "-x";
                string optStr = groups["opt "].Value;
                string[] opts = new string[] { "st", "d", "l", "stf" };



            }
        }

        public void Reset()
        {
        }
    }
}