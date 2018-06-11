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


        private List<LogEntry> _logEntries = new List<LogEntry>();
        private Dictionary<string, string> _debug = new Dictionary<string, string>();

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
            _row1.SetPixel(0, 0, new Color32(55, 55, 55, 255));
            _row1.Apply();

            _row2 = new Texture2D(1, 1);
            _row2.SetPixel(0, 0, new Color32(60, 60, 60, 255));
            _row2.Apply();

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

        void OnGUI()
        {
            InitializeOnce();
            var logEntryPosition = new Rect(0, 50, position.width, position.height - 100);
            RenderLogEntries(logEntryPosition);
            foreach (var kv in _debug)
            {
                EditorGUILayout.LabelField(kv.Key + "=" + kv.Value);
            }
        }

        void RenderLogEntries(Rect position)
        {
            int rowCount = Mathf.RoundToInt(position.height / _rowHeight);
            _debug["rowCount"] = rowCount.ToString();
            var scrollbarPosition = new Rect(position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth, position.y, GUI.skin.verticalScrollbar.fixedWidth, position.height);
            _scrollValue = GUI.VerticalScrollbar(scrollbarPosition, _scrollValue, 1, 0f, _logEntries.Count - rowCount);
            _debug["scroll"] = _scrollValue.ToString();

            int max = (int)_scrollValue + rowCount + 1;
            int i = (int)_scrollValue;
            int rowIndex = 0;
            for (; i < max; ++i, ++rowIndex)
            {
                Texture2D rowText = _row1;
                if (i % 2 == 0)
                {
                    rowText = _row2;
                }
                Rect rowRect = new Rect(position.x, position.y + rowIndex * _rowHeight, position.width - GUI.skin.verticalScrollbar.fixedWidth, _rowHeight);

                if (rowIndex >= rowCount) // last raw
                {
                    rowRect.height = _rowHeight - (position.height - (rowCount * _rowHeight));
                }
                GUI.DrawTexture(rowRect, rowText);
                EditorGUI.LabelField(rowRect, _logEntries[i].format);
            }
        }
    }
}