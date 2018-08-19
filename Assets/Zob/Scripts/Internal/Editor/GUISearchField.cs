using System.Collections.Generic;

namespace Zob.Internal.Editor
{
    internal class GUISearchField
    {
        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult;

        public GUISearchField()
        {
            _searchField = new UnityEditor.IMGUI.Controls.SearchField
            {
                autoSetFocusOnFindCommand = true
            };
        }

        public int OnGUI(int logEntryIndex, ILogEntryContainer entries)
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