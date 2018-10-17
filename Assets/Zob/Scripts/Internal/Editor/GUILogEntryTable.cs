﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal interface ITableLineCount
    {
        int Count { get; }
    }

    internal interface ITableLineRenderer
    {
        void OnGUI(Rect position, int index, int selectedIndex);
    }

    internal class GUILogEntryTable
    {
        private EditorWindow _parent;
        private float _scrollValue;
        private List<Rect> _entriesRect = new List<Rect>();
        private Texture2D _bottomBarTexture;
        private GUIVerticalSplit _split;
        private bool _enableScroll;
        private AutoScrollToSelected _autoScrollToSelected = new AutoScrollToSelected();
        private readonly ITableLineRenderer _renderer = null;

        // one scroll line is 10 unit in unity source ...
        private const float ScrollConstant = 10f;
        private const float RowHeight = 20f;


        public GUILogEntryTable(EditorWindow parent, ITableLineRenderer renderer)
        {
            _parent = parent;

            _bottomBarTexture = new Texture2D(1, 1);
            _bottomBarTexture.SetPixel(0, 0, new Color32(23, 23, 23, 255));
            _bottomBarTexture.Apply();
            _bottomBarTexture.hideFlags = HideFlags.HideAndDontSave;
            _renderer = renderer;
        }

        public void UpdateAutoScrolling(ITableLineCount lines)
        {
            int rowCount = GetRowCount();
            if (IsScrollCursorAtBottom(rowCount, lines.Count))
            {
                _scrollValue = lines.Count - rowCount;
            }
        }

        public int OnGUI(int selectedLogEntryIndex, ITableLineCount lines, bool scrollToSelected)
        {
            if (scrollToSelected && !_autoScrollToSelected.Enable)
            {
                _autoScrollToSelected.Enable = scrollToSelected;
            }
            if (Event.current.type == EventType.Used)
            {
                return selectedLogEntryIndex;
            }

            if (null == _split)
            {
                _split = new GUIVerticalSplit(_parent);
            }
            _split.OnGUI();

            // get the position from the vertical split
            Rect position = _split.Position;

            if (Event.current.type != EventType.Layout)
            {
                int rowCount = GetRowCount();
                _enableScroll = lines.Count > rowCount;
                bool scrollCursorAtBottom = IsScrollCursorAtBottom(rowCount, lines.Count);

                if (_enableScroll)
                {
                    var scrollbarPosition = new Rect(position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth,
                        position.y,
                        GUI.skin.verticalScrollbar.fixedWidth,
                        position.height);

                    _scrollValue = _autoScrollToSelected.Scroll(selectedLogEntryIndex, _scrollValue, rowCount);
                    _scrollValue = GUI.VerticalScrollbar(scrollbarPosition, _scrollValue, 1f, 0f, (lines.Count - rowCount + 1) * ScrollConstant);
                }

                _entriesRect.Clear();
                float delta = 0f;
                var totalHeight = rowCount * RowHeight;
                if (totalHeight > position.height)
                {
                    delta = totalHeight - position.height;
                }

                for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
                {
                    int index = Mathf.FloorToInt(_scrollValue / ScrollConstant) + rowIndex;
                    if (index < 0 || index >= lines.Count)
                    {
                        continue;
                    }

                    Rect rowRect = new Rect(
                        position.x,
                        position.y + rowIndex * RowHeight,
                        position.width,
                        RowHeight);

                    if (_enableScroll)
                    {
                        rowRect.width -= GUI.skin.verticalScrollbar.fixedWidth;
                    }

                    // set the row height and y depending on the position of the scroll
                    if (scrollCursorAtBottom)
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

                    _entriesRect.Add(rowRect);
                    _renderer.OnGUI(rowRect, index, selectedLogEntryIndex);
                }
            }

            // scrolling with scrollwheel
            if (_enableScroll && Event.current.type == EventType.ScrollWheel)
            {
                float step = Event.current.delta.y * ScrollConstant;
                if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
                {
                    step = Mathf.Sign(step) * lines.Count * 0.05f * ScrollConstant;
                }
                _scrollValue = Mathf.FloorToInt(Mathf.Clamp(_scrollValue + step, 0f, lines.Count * ScrollConstant));
                _parent.Repaint();
            }

            // log entry selection with mouse button
            if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                for (int i = 0; i < _entriesRect.Count; ++i)
                {
                    if (_entriesRect[i].Contains(Event.current.mousePosition))
                    {
                        selectedLogEntryIndex = i + Mathf.FloorToInt(_scrollValue / ScrollConstant);
                        _parent.Repaint();
                        break;
                    }
                }
            }

            // draw the bottom bar
            var bottomBarPosition = GUILayoutUtility.GetRect(0, 2);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(bottomBarPosition, _bottomBarTexture);
            }

            return selectedLogEntryIndex;
        }

        private int GetRowCount()
        {
            Rect position = _split.Position;
            int rowCount = Mathf.FloorToInt(position.height / RowHeight);
            return rowCount;
        }

        private bool IsScrollCursorAtBottom(int rowCount, int lineCount)
        {
            return rowCount + Mathf.FloorToInt(_scrollValue / ScrollConstant) == lineCount;
        }

        class AutoScrollToSelected
        {
            public bool Enable { get; set; }

            public float Scroll(int selectedLogEntryIndex, float scrollValue, int rowCount)
            {
                if (!Enable)
                {
                    return scrollValue;
                }
                if (selectedLogEntryIndex == -1)
                {
                    return scrollValue;
                }
                Enable = false;
                int minEntryIndex = Mathf.FloorToInt(scrollValue / ScrollConstant);
                int maxEntryIndex = Mathf.FloorToInt(scrollValue / ScrollConstant) + rowCount;
                if (selectedLogEntryIndex < minEntryIndex || selectedLogEntryIndex >= maxEntryIndex)
                {
                    return (selectedLogEntryIndex - Mathf.FloorToInt(rowCount / 2f)) * ScrollConstant;
                }
                return scrollValue;
            }
        }
    }
}