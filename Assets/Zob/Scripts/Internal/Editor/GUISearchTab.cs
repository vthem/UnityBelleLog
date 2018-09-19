using System.Collections.Generic;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class GUISearchTab
    {
        public bool HasUpdatedLogEntryIndex { get; protected set; }

        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult;
        private string _searchFieldContent;
        private GUIStyle _toolbarButtonStyle;
        private bool _searchFound = false;

        protected enum SearchDirection
        {
            Backward = -1,
            None = 0,
            Forward = 1
        }

        public GUISearchTab()
        {

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
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            _searchFieldContent = _searchField.OnToolbarGUI(_searchFieldContent);
            if (_searchFieldContent != _searchFieldResult)
            {
                _searchFieldResult = _searchFieldContent;
                _searchFound = Search(entries, SearchDirection.None, SearchDirection.Forward, ref logEntryIndex);
            }

            GUILayout.Space(5f);
            var maxWidth = GUILayout.MaxWidth(20);
            GUI.enabled = _searchFound;
            if (GUILayout.Button("<", _toolbarButtonStyle, maxWidth))
            {
                SearchBackward(entries, ref logEntryIndex);
            }
            if (GUILayout.Button(">", _toolbarButtonStyle, maxWidth))
            {
                SearchForward(entries, ref logEntryIndex);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            return logEntryIndex;
        }

        protected void SearchForward(ILogEntryContainer entries, ref int logEntryIndex)
        {
            _searchFound = Search(entries, SearchDirection.Forward, SearchDirection.Forward, ref logEntryIndex);
        }

        protected void SearchBackward(ILogEntryContainer entries, ref int logEntryIndex)
        {
            _searchFound = Search(entries, SearchDirection.Backward, SearchDirection.Backward, ref logEntryIndex);
        }

        protected bool Search(ILogEntryContainer entries, SearchDirection initialDirection, SearchDirection direction, ref int logEntryIndex)
        {
            if (string.IsNullOrEmpty(_searchFieldResult))
            {
                return false;
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
                if (entries[cur].content.Contains(_searchFieldResult))
                {
                    HasUpdatedLogEntryIndex = true;
                    logEntryIndex = cur;
                    return true;
                }
                cur = CycleCursor(cur + (int)direction, entries.Count);
                loop = cur == start;
            }
            return false;
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