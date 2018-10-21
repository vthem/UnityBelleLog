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
        private GUIStyle _toolbarButtonStyle;
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
            if (null == _toolbarButtonStyle)
            {
                _toolbarButtonStyle = new GUIStyle("ToolbarButton");
            }

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

            bool searchBackward = GUILayout.Button("<", _toolbarButtonStyle, maxWidth)
                || Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.F3 && Event.current.shift
                || !EditorGUIUtility.editingTextField && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.P;
            if (searchBackward)
            {
                logEntryIndex = SearchBackward(entries, logEntryIndex);
                HasUpdatedLogEntryIndex = true;
                _parent.Repaint();
            }

            bool searchForward = GUILayout.Button(">", _toolbarButtonStyle, maxWidth)
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

        protected int SearchForward(ILogEntryContainer entries, int logEntryIndex)
        {
            return Search(entries, SearchDirection.Forward, SearchDirection.Forward, logEntryIndex);
        }

        protected int SearchBackward(ILogEntryContainer entries, int logEntryIndex)
        {
            return Search(entries, SearchDirection.Backward, SearchDirection.Backward, logEntryIndex);
        }

        protected int Search(ILogEntryContainer entries, SearchDirection initialDirection, SearchDirection direction, int logEntryIndex)
        {
            if (string.IsNullOrEmpty(_searchFieldResult))
            {
                return -1;
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
                if (entries[cur].content.IndexOf(_searchFieldResult, _matchCase ? System.StringComparison.Ordinal : System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return cur;
                }
                cur = CycleCursor(cur + (int)direction, entries.Count);
                loop = cur == start;
            }
            return -1;
        }

        protected static int CycleCursor(int cursor, int count)
        {
            if (cursor < 0)
            {
                cursor = count - 1;
            }
            return cursor % count;
        }
    }
}