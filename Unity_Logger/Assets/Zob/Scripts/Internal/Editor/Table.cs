using System;
using System.Collections.Generic;
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

    internal class Table
    {
        private EditorWindow _parent;
        private float _scrollValue;
        private List<Rect> _entriesRect = new List<Rect>();
        private Texture2D _bottomBarTexture;
        private VerticalSplit _split;
        private AutoScrollToSelected _autoScrollToSelected = new AutoScrollToSelected();
        private readonly ITableLineRenderer _renderer = null;
        private bool _onGUIInitialized = false;
        private DateTime? _lastClick;
        private int _lastClickIndex;

        // one scroll line is 10 unit in unity source ...
        private const float ScrollConstant = 10f;
        private const float RowHeight = 20f;

        public bool HasUpdatedSelectedEntry { get; protected set; }
        public bool HasDoubleClickedEntry { get; protected set; }

        public Table(EditorWindow parent, ITableLineRenderer renderer)
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
            if (!_onGUIInitialized)
            {
                return;
            }

            int rowCount = GetRowCount();
            if (IsScrollCursorAtBottom(rowCount, lines.Count))
            {
                _scrollValue = lines.Count - rowCount;
            }
        }

        public int OnGUI(int selectedEntry, ITableLineCount lines, bool scrollToSelected)
        {
            OnGUIInitialize();

            HasUpdatedSelectedEntry = false;
            HasDoubleClickedEntry = false;

            if (scrollToSelected && !_autoScrollToSelected.Enable)
            {
                _autoScrollToSelected.Enable = scrollToSelected;
            }
            if (Event.current.type == EventType.Used)
            {
                return selectedEntry;
            }

            _split.OnGUI();

            // get the position from the vertical split
            Rect position = _split.Position;

            // we don't use GUILayout in the following part :
            // compute each row position
            int rowCount = GetRowCount();
            bool enableScroll = lines.Count > rowCount;
            if (Event.current.type != EventType.Layout)
            {
                bool scrollCursorAtBottom = IsScrollCursorAtBottom(rowCount, lines.Count);

                if (enableScroll)
                {
                    var scrollbarPosition = new Rect(position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth,
                        position.y,
                        GUI.skin.verticalScrollbar.fixedWidth,
                        position.height);

                    _scrollValue = _autoScrollToSelected.Scroll(selectedEntry, _scrollValue, rowCount);
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
                    Rect rowRect = new Rect(
                        position.x,
                        position.y + rowIndex * RowHeight,
                        position.width,
                        RowHeight);

                    if (enableScroll)
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
                    _renderer.OnGUI(rowRect, Mathf.FloorToInt(_scrollValue / ScrollConstant) + rowIndex, selectedEntry);
                }
            }

            // scrolling with scrollwheel
            if (enableScroll && Event.current.type == EventType.ScrollWheel)
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
                        var newIndex = i + Mathf.FloorToInt(_scrollValue / ScrollConstant);
                        if (newIndex >= 0 && newIndex < lines.Count)
                        {
                            _parent.Repaint();
                            HasUpdatedSelectedEntry = true;
                            selectedEntry = newIndex;

                            if (_lastClick.HasValue)
                            {
                                int deltaTime = (DateTime.Now - _lastClick.Value).Milliseconds;
                                bool isDoubleClick = deltaTime < 500;
                                if (selectedEntry == _lastClickIndex && isDoubleClick)
                                {
                                    HasDoubleClickedEntry = true;
                                }
                            }
                            _lastClick = DateTime.Now;
                            _lastClickIndex = selectedEntry;
                        }
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

            return selectedEntry;
        }

        protected void OnGUIInitialize()
        {
            if (_onGUIInitialized)
            {
                return;
            }

            if (null == _split)
            {
                _split = new VerticalSplit(_parent);
            }

            _onGUIInitialized = true;
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

            public float Scroll(int selectedEntry, float scrollValue, int rowCount)
            {
                if (!Enable)
                {
                    return scrollValue;
                }
                if (selectedEntry == -1)
                {
                    return scrollValue;
                }
                Enable = false;
                int minEntryIndex = Mathf.FloorToInt(scrollValue / ScrollConstant);
                int maxEntryIndex = Mathf.FloorToInt(scrollValue / ScrollConstant) + rowCount;
                if (selectedEntry < minEntryIndex || selectedEntry >= maxEntryIndex)
                {
                    return (selectedEntry - Mathf.FloorToInt(rowCount / 2f)) * ScrollConstant;
                }
                return scrollValue;
            }
        }
    }
}