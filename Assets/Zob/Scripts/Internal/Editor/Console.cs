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
        private Texture2D _selectedRowTexture;


        private List<LogEntry> _logEntries = new List<LogEntry>();
        private Dictionary<string, string> _debug = new Dictionary<string, string>();
        private int _selectedLogEntryIndex = -1;
        private SearchField _searchField;
        private LogEntryArray _logEntryArray;

        public ConsoleConfig Config { get { return _config; } }

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
            _searchField = new SearchField();
            _logEntryArray = new LogEntryArray(this);

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


        bool collapse;
        protected void OnGUI()
        {
            InitializeOnce();

            foreach (var kv in _debug)
            {
                EditorGUILayout.LabelField(kv.Key + "=" + kv.Value);
            }

            var toolbarPosition = new Rect(0, 0, position.width, GUI.skin.button.fixedHeight);
            GUI.BeginGroup(toolbarPosition, EditorStyles.toolbar);
            collapse = GUILayout.Toggle(collapse, new GUIContent("Collapse"), EditorStyles.toolbarButton, GUILayout.Width(50));
            GUI.EndGroup();

            var searchFieldPosition = new Rect(100, 0, position.width - 120, 20f);
            _selectedLogEntryIndex = _searchField.OnGUI(searchFieldPosition, _selectedLogEntryIndex, _logEntries);

            var logEntryPosition = new Rect(0, toolbarPosition.height, position.width, position.height - 100);
            _selectedLogEntryIndex = _logEntryArray.OnGUI(logEntryPosition, _selectedLogEntryIndex, _logEntries);
        }
    }
}