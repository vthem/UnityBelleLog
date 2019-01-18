#if UNITY_EDITOR
using BelleLog.Internal.Editor.Filter;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogFilterInputRenderer : ILogFilter, ILogFilterEnableChangedEvent
    {
        enum Opt
        {
            StackTrace,
            StackTraceFile,
            Domain,
            Content
        }

        private string _filterInputText;
        private LogFilterChain _filterChain = new LogFilterChain();

        private readonly Dictionary<string, Opt> _strToOpt = new Dictionary<string, Opt>
        {
            {"st", Opt.StackTrace },
            {"d", Opt.Domain },
            {"stf", Opt.StackTraceFile }
        };

        public event Action FilterEnableChanged;

        public LogFilterInputRenderer()
        {
            Enable = false;
        }

        public bool Enable { get; set; }

        public void Apply(LogEntry logEntry, ref LogFilterAction action, out LogFilterTermination termination)
        {
            action = _filterChain.Apply(logEntry);
            termination = LogFilterTermination.Continue;
        }

        public void OnGUI()
        {
            bool returnKeyDown = Event.current.control && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter: ", GUILayout.Width(40));
            _filterInputText = EditorGUILayout.TextArea(_filterInputText);
            if (returnKeyDown || GUILayout.Button("V", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                ParseInputText();
            }
            if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20)))
            {
                _filterInputText = string.Empty;
                ParseInputText();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void ParseInputText()
        {
            Enable = false;

            List<ILogFilter> filters = new List<ILogFilter>();

            if (!string.IsNullOrEmpty(_filterInputText))
            {
                var matches = Regex.Matches(_filterInputText, @"(?<cmd>-[ei]):?(?<opt>[a-z]*) (('(?<v1>(.*))')|(?<v2>([^ ]*)))");
                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;
                    var v1 = groups["v1"].Value;
                    var v2 = groups["v2"].Value;
                    var val = v1;
                    if (string.IsNullOrEmpty(v1))
                    {
                        val = v2;
                    }

                    bool exclude = groups["cmd"].Value == "-e";
                    Opt opt;
                    if (!_strToOpt.TryGetValue(groups["opt"].Value, out opt))
                    {
                        opt = Opt.Content;
                    }

                    filters.Add(CreateFilter(exclude, opt, val));
                }
            }

            _filterChain.RemoveAll();
            for (int i = 0; i < filters.Count; ++i)
            {
                _filterChain.AddFilter(filters[i]);
            }
            Enable = true;
            if (FilterEnableChanged != null)
            {
                FilterEnableChanged.Invoke();
            }
        }

        private ILogFilter CreateFilter(bool exclude, Opt opt, string value)
        {
            PredicateLogFilter filter;
            if (exclude)
            {
                filter = new PredicateLogFilter
                {
                    TrueTermination = LogFilterTermination.Stop,
                    FalseTermination = LogFilterTermination.Continue,
                    TrueAction = LogFilterAction.Reject,
                    FalseAction = LogFilterAction.Accept,
                    Enable = true
                };
            }
            else
            {
                filter = new PredicateLogFilter
                {
                    TrueTermination = LogFilterTermination.Stop,
                    FalseTermination = LogFilterTermination.Continue,
                    TrueAction = LogFilterAction.Accept,
                    FalseAction = LogFilterAction.Reject,
                    Enable = true
                };
            }

            switch (opt)
            {
                case Opt.StackTrace:
                    filter.Predicate = (entry) =>
                    {
                        for (int i = 0; i < entry.stackTrace.Length; ++i)
                        {
                            if (entry.stackTrace[i].className.Contains(value) || entry.stackTrace[i].methodName.Contains(value))
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                    break;
                case Opt.StackTraceFile:
                    filter.Predicate = (entry) =>
                    {
                        for (int i = 0; i < entry.stackTrace.Length; ++i)
                        {
                            if (entry.stackTrace[i].fileName.Contains(value))
                            {
                                return true;
                            }
                        }
                        return false;
                    };
                    break;
                case Opt.Domain:
                    filter.Predicate = (entry) =>
                    {
                        return entry.domain.Contains(value);
                    };
                    break;
                case Opt.Content:
                default:
                    filter.Predicate = (entry) =>
                    {
                        return entry.content.Contains(value);
                    };
                    break;
            }
            return filter;
        }

        public void Reset()
        {
        }
    }
}
#endif