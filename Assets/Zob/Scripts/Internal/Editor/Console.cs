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
        private const float TabHeight = 18f;

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

            _separationBar = new SeparationBarGUI(2 / 3f);
        }

        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private bool _showFilter;

        private SeparationBarGUI _separationBar;

        //protected void OnGUI()
        //{
        //    InitializeOnce();

        //    foreach (var kv in _debug)
        //    {
        //        EditorGUILayout.LabelField(kv.Key + "=" + kv.Value);
        //    }

        //    var toolbarPosition = new Rect(0, 0, position.width, TabHeight);
        //    GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

        //    var clearPosition = toolbarPosition;
        //    clearPosition.width = 40f;
        //    clearPosition.x = 5f;
        //    if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
        //    {
        //    }

        //    EditorGUILayout.Space();

        //    var collapsePostion = clearPosition;
        //    collapsePostion.x = clearPosition.x + clearPosition.width + 5f;
        //    collapsePostion.width = 60f;
        //    collapse = GUILayout.Toggle(collapse, "Collapse", new GUIStyle("ToolbarButton"));
        //    clearOnPlay = GUILayout.Toggle(clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
        //    errorPause = GUILayout.Toggle(errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
        //    GUILayout.FlexibleSpace();
        //    showFilter = GUILayout.Toggle(showFilter, "Show filter", new GUIStyle("ToolbarButton"));
        //    GUILayout.EndHorizontal();

        //    //GUILayout.BeginHorizontal();
        //    GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));
        //    _selectedLogEntryIndex = _searchField.OnGUI(_selectedLogEntryIndex, _logEntries);
        //    GUILayout.Space(5f);
        //    var maxWidth = GUILayout.MaxWidth(20);
        //    if (GUILayout.Button("<", new GUIStyle("ToolbarButton"), maxWidth))
        //    {
        //    }
        //    if (GUILayout.Button(">", new GUIStyle("ToolbarButton"), maxWidth))
        //    {
        //    }
        //    GUILayout.EndHorizontal();


        //    //var searchFieldPosition = new Rect(100, 0, position.width - 120, 20f);

        //    //var logEntryPosition = new Rect(0, toolbarPosition.height, position.width, position.height - 100);
        //    var logEntryPosition = GUILayoutUtility.GetRect(position.width, position.height - 100);
        //    _selectedLogEntryIndex = _logEntryArray.OnGUI(logEntryPosition, _selectedLogEntryIndex, _logEntries);


        //}

        protected void OnGUI()
        {
            _separationBar.Render(position.width, position.height);
        }
    }
}