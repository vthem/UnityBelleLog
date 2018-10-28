using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class SearchTab
    {
        public bool HasUpdatedLogEntryIndex { get; protected set; }

        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult;
        private string _searchFieldContent;
        private int _searchResult;
        private bool _matchCase = false;
        private readonly EditorWindow _parent;
        private string _text = string.Empty;

        protected enum SearchDirection
        {
            Backward = -1,
            None = 0,
            Forward = 1
        }

        public SearchTab(EditorWindow parent)
        {
            _parent = parent;
            _searchField = new UnityEditor.IMGUI.Controls.SearchField
            {
                autoSetFocusOnFindCommand = true
            };
        }

        public int OnGUI(int logEntryIndex, ILogEntryContainer entries)
        {
            HasUpdatedLogEntryIndex = false;
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            // catch the return / enter key before calling OnToolBarGUI
            // otherwise the event is used
            bool returnKeyDown = EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return);
            _searchFieldContent = _searchField.OnToolbarGUI(_searchFieldContent);
            if (_searchFieldContent != _searchFieldResult)
            {
                _searchFieldResult = _searchFieldContent;
                _searchResult = Search(entries, SearchDirection.None, SearchDirection.Forward, logEntryIndex);
            }

            GUILayout.Space(5f);
            var maxWidth = GUILayout.MaxWidth(20);
            GUI.enabled = _searchResult != -1;

            bool searchBackward = GUILayout.Button("<", EditorStyles.toolbarButton, maxWidth)
                || Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F3 && Event.current.shift
                || !EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.P;
            if (searchBackward)
            {
                logEntryIndex = SearchBackward(entries, logEntryIndex);
                HasUpdatedLogEntryIndex = true;
                _parent.Repaint();
            }

            bool searchForward = GUILayout.Button(">", EditorStyles.toolbarButton, maxWidth)
                || returnKeyDown
                || Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F3
                || !EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.N;
            if (searchForward)
            {
                logEntryIndex = SearchForward(entries, logEntryIndex);
                HasUpdatedLogEntryIndex = true;
                _parent.Repaint();
            }
            GUI.enabled = true;
            _matchCase = GUILayout.Toggle(_matchCase, "Match Case", EditorStyles.toolbarButton, GUILayout.Width(70));
            GUILayout.EndHorizontal();
            return logEntryIndex;
        }

        private int SearchForward(ILogEntryContainer entries, int logEntryIndex)
        {
            return Search(entries, SearchDirection.Forward, SearchDirection.Forward, logEntryIndex);
        }

        private int SearchBackward(ILogEntryContainer entries, int logEntryIndex)
        {
            return Search(entries, SearchDirection.Backward, SearchDirection.Backward, logEntryIndex);
        }

        private int Search(ILogEntryContainer entries, SearchDirection initialDirection, SearchDirection direction, int logEntryIndex)
        {
            if (string.IsNullOrEmpty(_searchFieldResult)
                || entries.Count == 0)
            {
                return -1;
            }

            Func<LogEntry, bool> matchFunc = SearchContent;
            if (SearchFieldResultContainMetaLevel())
            {
                matchFunc = SearchLogLevel;
            }

            int start = logEntryIndex;
            if (start == -1)
            {
                start = 0;
            }
            start = CycleCursor(start + (int)initialDirection, entries.Count);
            bool loop = false;
            int cur = start;
            while (!loop)
            {
                if (matchFunc(entries[cur]))
                {
                    return cur;
                }
                cur = CycleCursor(cur + (int)direction, entries.Count);
                loop = cur == start;
            }
            return -1;
        }

        private bool SearchLogLevel(LogEntry entry)
        {
            if (_searchFieldResult.Length < 3)
            {
                return false;
            }
            var meta = _searchFieldResult[2];
            if (meta == 't' && entry.level == LogLevel.Trace) { return true; }
            else if (meta == 'd' && entry.level == LogLevel.Debug) { return true; }
            else if (meta == 'i' && entry.level == LogLevel.Info) { return true; }
            else if (meta == 'w' && entry.level == LogLevel.Warning) { return true; }
            else if (meta == 'e' && entry.level == LogLevel.Error) { return true; }
            else if (meta == 'f' && entry.level == LogLevel.Fatal) { return true; }
            return false;
        }

        private bool SearchContent(LogEntry entry)
        {
            return entry.content.IndexOf(_searchFieldResult, _matchCase ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool SearchFieldResultContainMetaLevel()
        {
            return _searchFieldResult.IndexOf(":") == 1
                && _searchFieldResult[0] == 'l';
        }

        private static int CycleCursor(int cursor, int count)
        {
            if (cursor < 0)
            {
                cursor = count - 1;
            }
            return cursor % count;
        }
    }
}