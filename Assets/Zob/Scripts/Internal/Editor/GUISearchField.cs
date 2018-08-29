﻿using System.Collections.Generic;

namespace Zob.Internal.Editor
{
    internal class GUISearchField
    {
        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult;
        private string _searchFieldContent;

        protected enum SearchDirection
        {
            Forward = 1,
            Backward = -1
        }

        public GUISearchField()
        {
            _searchField = new UnityEditor.IMGUI.Controls.SearchField
            {
                autoSetFocusOnFindCommand = true
            };
        }

        public int OnGUI(int logEntryIndex, ILogEntryContainer entries)
        {
            _searchFieldContent = _searchField.OnToolbarGUI(_searchFieldContent);
            bool hasFocus = _searchField.HasFocus();
            if (!hasFocus && _searchFieldContent != _searchFieldResult)
            {
                _searchFieldResult = _searchFieldContent;
                return SearchForward(logEntryIndex, entries);
            }
            return logEntryIndex;
        }

        public int SearchForward(int logEntryIndex, ILogEntryContainer entries)
        {
            return Search(logEntryIndex, entries, SearchDirection.Forward);
        }

        public int SearchBackward(int logEntryIndex, ILogEntryContainer entries)
        {
            return Search(logEntryIndex, entries, SearchDirection.Backward);
        }

        protected int Search(int logEntryIndex, ILogEntryContainer entries, SearchDirection direction)
        {
            if (string.IsNullOrEmpty(_searchFieldResult))
            {
                return logEntryIndex;
            }

            int start = logEntryIndex;
            if (start == -1)
            {
                start = 0;
            }
            start = CycleCursor(start + (int)direction, entries.Count);
            bool loop = false;
            int cur = start;
            while (!loop)
            {
                if (entries[cur].format.Contains(_searchFieldResult))
                {
                    logEntryIndex = cur;
                    break;
                }
                cur = CycleCursor(cur + (int)direction, entries.Count);
                loop = cur == start;
            }
            return logEntryIndex;
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