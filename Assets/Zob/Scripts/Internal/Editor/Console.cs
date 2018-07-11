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
        private const float TabHeight = 18f;

        private Texture2D _row1;
        private Texture2D _row2;
        private Texture2D _selectedRowTexture;

        private List<LogEntry> _logEntries = new List<LogEntry>();
        private int _selectedLogEntryIndex = -1;
        private SearchField _searchField;
        private LogEntryArray _logEntryArray;
        private bool _initialized = false;
        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private bool _showFilter;
        private SeparationBarGUI _separationBar;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.Show();
            Debug.Log("open zop console window");
        }

        public Console()
        {

            Debug.Log("Console " + _initialized + " id=" + GetInstanceID());
        }

        void OnEnable()
        {
            Initialize();
        }

        void OnDisable()
        {
            _initialized = false;
        }

        private void Initialize()
        {
            Debug.Log("InitializeOnce " + _initialized + " id=" + GetInstanceID());
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            Debug.Log("InitializeOnce...");

            wantsMouseMove = true;
            titleContent = new GUIContent("ZobConsole");
            Debug.Log("InitializeOnce...");
            _logEntryArray = new LogEntryArray(this);
            Debug.Log("InitializeOnce...");
            _separationBar = new SeparationBarGUI(2 / 3f, this);
            Debug.Log("InitializeOnce...");
            _searchField = new SearchField();
            Debug.Log("InitializeOnce...");

            for (int i = 0; i < 1000; ++i)
            {
                var entry = new LogEntry();
                entry.args = null;
                entry.domain = "log-console";
                entry.format = "this is a log entry [" + i + "]";
                entry.level = LogLevel.Info;
                entry.timestamp = DateTime.Now;
                _logEntries.Add(entry);
            }
            Debug.Log("InitializeOnce... complete");

            Repaint();

        }

        protected void OnGUI()
        {
            if (!_initialized)
            {
                EditorGUILayout.LabelField("Not properly initialized");
                return;
            }
            var toolbarPosition = new Rect(0, 0, position.width, TabHeight);
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
            {
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            _collapse = GUILayout.Toggle(_collapse, "Collapse", new GUIStyle("ToolbarButton"));
            _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
            _errorPause = GUILayout.Toggle(_errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
            GUILayout.FlexibleSpace();
            _showFilter = GUILayout.Toggle(_showFilter, "Show filter", new GUIStyle("ToolbarButton"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));
            _selectedLogEntryIndex = _searchField.OnGUI(_selectedLogEntryIndex, _logEntries);
            GUILayout.Space(5f);
            var maxWidth = GUILayout.MaxWidth(20);
            if (GUILayout.Button("<", new GUIStyle("ToolbarButton"), maxWidth))
            {
            }
            if (GUILayout.Button(">", new GUIStyle("ToolbarButton"), maxWidth))
            {
            }
            GUILayout.EndHorizontal();

            var cur = GUILayoutUtility.GetRect(0, 0, 0, 0);
            var logEntryPosition = GUILayoutUtility.GetRect(position.width, _separationBar.Y);
            logEntryPosition.height = logEntryPosition.height - cur.y;
            _selectedLogEntryIndex = _logEntryArray.OnGUI(logEntryPosition, _selectedLogEntryIndex, _logEntries);
            _separationBar.Render(position.width, cur.y, position.height);

            if (_selectedLogEntryIndex != -1)
            {
                EditorGUILayout.SelectableLabel(_logEntries[_selectedLogEntryIndex].format);
            }
        }
    }
}