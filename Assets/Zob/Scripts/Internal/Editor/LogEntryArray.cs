using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class LogEntryArray
    {
        private Console _console;
        private float _scrollValue;
        private List<Rect> _entriesRect = new List<Rect>();

        private Texture2D _rowTexture1;
        private Texture2D _rowTexture2;
        private Texture2D _selectedRowTexture;

        private const float _rowHeight = 20f;

        public LogEntryArray(Console console)
        {
            _console = console;

            _rowTexture1 = new Texture2D(1, 1);
            _rowTexture1.SetPixel(0, 0, new Color32(55, 125, 55, 255));
            _rowTexture1.Apply();

            _rowTexture2 = new Texture2D(1, 1);
            _rowTexture2.SetPixel(0, 0, new Color32(120, 60, 60, 255));
            _rowTexture2.Apply();

            _selectedRowTexture = new Texture2D(1, 1);
            _selectedRowTexture.SetPixel(0, 0, new Color32(62, 95, 150, 255));
            _selectedRowTexture.Apply();
        }

        public int OnGUI(Rect position, int selectedLogEntryIndex, List<LogEntry> logEntries)
        {
            if (Event.current.type != EventType.Layout)
            {
                int rowCount = Mathf.CeilToInt(position.height / _rowHeight);
                var scrollbarPosition = new Rect(
                    position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth,
                    position.y,
                    GUI.skin.verticalScrollbar.fixedWidth,
                    position.height);
                _scrollValue = GUI.VerticalScrollbar(scrollbarPosition, _scrollValue, 1, 0f, logEntries.Count - rowCount + 1);

                _entriesRect.Clear();
                float delta = 0f;
                var totalHeight = rowCount * _rowHeight;
                if (totalHeight > position.height)
                {
                    delta = totalHeight - position.height;
                }

                for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
                {
                    Texture2D rowTexture = _rowTexture1;
                    if (((int)_scrollValue + rowIndex) % 2 == 0)
                    {
                        rowTexture = _rowTexture2;
                    }
                    if (rowIndex + (int)_scrollValue == selectedLogEntryIndex)
                    {
                        rowTexture = _selectedRowTexture;
                    }
                    Rect rowRect = new Rect(
                        position.x,
                        position.y + rowIndex * _rowHeight,
                        position.width - GUI.skin.verticalScrollbar.fixedWidth,
                        _rowHeight);
                    if (rowCount + (int)_scrollValue == logEntries.Count)
                    {
                        // scroll cursor is at the bottom
                        if (rowIndex == 0)
                        {
                            rowRect.height = rowRect.height - delta;
                        }
                        else
                        {
                            rowRect.y = rowRect.y - delta;
                        }
                    }
                    else
                    {
                        if (rowIndex == rowCount - 1) // last raw
                        {
                            rowRect.height = rowRect.height - delta;
                        }
                    }

                    _entriesRect.Add(rowRect);
                    GUI.DrawTexture(rowRect, rowTexture);
                    EditorGUI.LabelField(rowRect, logEntries[(int)_scrollValue + rowIndex].format);
                }
            }

            if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                selectedLogEntryIndex = -1;
                for (int i = 0; i < _entriesRect.Count; ++i)
                {
                    if (_entriesRect[i].Contains(Event.current.mousePosition))
                    {
                        selectedLogEntryIndex = i + (int)_scrollValue;
                        _console.Repaint();
                        break;
                    }
                }
            }

            return selectedLogEntryIndex;
        }
    }
}