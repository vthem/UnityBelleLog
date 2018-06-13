using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class Console : EditorWindow
    {
        private Vector2 scrollPos;
        private int count = 0;
        private int texSize = 10;
        private Rect _rect;
        private float _scrollValue;
        private float _rowHeight = 20f;
        private bool _initialized = false;
        private ConsoleConfig _config;

        private Texture2D _row1;
        private Texture2D _row2;
        private Texture2D _selectedRow;


        private List<LogEntry> _logEntries = new List<LogEntry>();
        private Dictionary<string, string> _debug = new Dictionary<string, string>();
        private List<Rect> _entriesRect = new List<Rect>();
        private int _selectedRowEntryIndex = -1;
        private UnityEditor.IMGUI.Controls.SearchField _searchField;
        private string _searchFieldResult = "...";

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.InitializeOnce();
            console.Show();
            Debug.Log("open zop console window");
            Debug.Log("open zop console window x2");
        }

        private void InitializeOnce()
        {
            if (_initialized)
            {
                return;
            }
            wantsMouseMove = true;
            titleContent = new GUIContent("ZobConsole");
            _config = ConsoleConfig.Load();
            _initialized = true;
            _row1 = new Texture2D(1, 1);
            _row1.SetPixel(0, 0, new Color32(155, 155, 155, 255));
            _row1.Apply();

            _row2 = new Texture2D(1, 1);
            _row2.SetPixel(0, 0, new Color32(60, 60, 60, 255));
            _row2.Apply();

            _selectedRow = new Texture2D(1, 1);
            _selectedRow.SetPixel(0, 0, new Color32(62, 95, 150, 255));
            _selectedRow.Apply();

            _searchField = new UnityEditor.IMGUI.Controls.SearchField();
            _searchField.autoSetFocusOnFindCommand = true;

            for (int i = 0; i < 100; ++i)
            {
                var entry = new LogEntry();
                entry.args = null;
                entry.domain = "log-console";
                entry.format = "this is a log entry [" + i + "]";
                entry.level = LogLevel.Info;
                entry.timestamp = DateTime.Now;
                _logEntries.Add(entry);
            }
        }

        protected void OnGUI()
        {
            InitializeOnce();

            foreach (var kv in _debug)
            {
                EditorGUILayout.LabelField(kv.Key + "=" + kv.Value);
            }

            if (Event.current.type != EventType.Layout)
            {
                var searchFieldPosition = new Rect(100, 2, position.width - 120, 20f);
                _searchFieldResult = _searchField.OnToolbarGUI(searchFieldPosition, _searchFieldResult);

                var logEntryPosition = new Rect(0, 50, position.width, position.height - 100);
                RenderLogEntries(logEntryPosition);
            }
            if (Event.current.keyCode == KeyCode.Space && Event.current.type == EventType.KeyDown)
            {
                _scrollValue += 5;
                Repaint();
            }

            if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
            {
                HandleLeftMouseClick();
            }
        }

        protected void RenderLogEntries(Rect position)
        {
            int rowCount = Mathf.CeilToInt(position.height / _rowHeight);
            _debug["rowCount"] = rowCount.ToString();
            var scrollbarPosition = new Rect(position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth, position.y, GUI.skin.verticalScrollbar.fixedWidth, position.height);
            _scrollValue = GUI.VerticalScrollbar(scrollbarPosition, _scrollValue, 1, 0f, _logEntries.Count - rowCount + 1);
            _debug["scroll"] = _scrollValue.ToString();

            _entriesRect.Clear();
            for (int rowIndex = 0; rowIndex < rowCount; ++rowIndex)
            {
                Texture2D rowTexture = _row1;
                if (((int)_scrollValue + rowIndex) % 2 == 0)
                {
                    rowTexture = _row2;
                }
                if (rowIndex + (int)_scrollValue == _selectedRowEntryIndex)
                {
                    rowTexture = _selectedRow;
                }
                Rect rowRect = new Rect(position.x, position.y + rowIndex * _rowHeight, position.width - GUI.skin.verticalScrollbar.fixedWidth, _rowHeight);

                if (rowIndex == rowCount - 1) // last raw
                {
                    var totalHeight = rowCount * _rowHeight;
                    if (totalHeight > position.height)
                    {
                        rowRect.height = _rowHeight - (totalHeight - position.height);
                    }
                }
                _entriesRect.Add(rowRect);
                GUI.DrawTexture(rowRect, rowTexture);
                EditorGUI.LabelField(rowRect, _logEntries[(int)_scrollValue + rowIndex].format);
            }
        }

        protected void HandleLeftMouseClick()
        {
            Debug.Log("click=" + Event.current.mousePosition);
            _selectedRowEntryIndex = -1;
            for (int i = 0; i < _entriesRect.Count; ++i)
            {
                if (_entriesRect[i].Contains(Event.current.mousePosition))
                {
                    _selectedRowEntryIndex = i + (int)_scrollValue;
                    Repaint();
                    break;
                }
            }
        }
    }
}