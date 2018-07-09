using System.Collections.Generic;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class SearchField
    {
        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult;

        public SearchField()
        {
            _searchField = new UnityEditor.IMGUI.Controls.SearchField();
            _searchField.autoSetFocusOnFindCommand = true;
        }

        public int OnGUI(int logEntryIndex, List<LogEntry> entries)
        {
            var newSearchFieldResult = _searchField.OnToolbarGUI(_searchFieldResult);
            if (newSearchFieldResult != _searchFieldResult)
            {
                _searchFieldResult = newSearchFieldResult;
                int start = logEntryIndex;
                if (start == -1)
                {
                    start = 0;
                }
                bool loop = false;
                int cur = start;
                while (!loop)
                {
                    if (entries[cur].format.Contains(_searchFieldResult))
                    {
                        logEntryIndex = cur;
                        break;
                    }
                    cur = (cur + 1) % entries.Count;
                    loop = cur == start;
                }
            }
            return logEntryIndex;
        }
    }
}