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

            _separationBar = new SeparationBarGUI(2 / 3f, this);
        }

        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private bool _showFilter;

        private SeparationBarGUI _separationBar;

        protected void OnGUI()
        {
            InitializeOnce();

            foreach (var kv in _debug)
            {
                EditorGUILayout.LabelField(kv.Key + "=" + kv.Value);
            }

            var toolbarPosition = new Rect(0, 0, position.width, TabHeight);
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            //var clearPosition = toolbarPosition;
            //clearPosition.width = 40f;
            //clearPosition.x = 5f;
            if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
            {
            }

            EditorGUILayout.Space();

            //var collapsePostion = clearPosition;
            //collapsePostion.x = clearPosition.x + clearPosition.width + 5f;
            //collapsePostion.width = 60f;
            _collapse = GUILayout.Toggle(_collapse, "Collapse", new GUIStyle("ToolbarButton"));
            _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
            _errorPause = GUILayout.Toggle(_errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
            GUILayout.FlexibleSpace();
            _showFilter = GUILayout.Toggle(_showFilter, "Show filter", new GUIStyle("ToolbarButton"));
            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();
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
            DebugConsole.SetValue("cur", cur.ToString());
            DebugConsole.SetValue("height", (_separationBar.Y - cur.y).ToString());
            var logEntryPosition = GUILayoutUtility.GetRect(position.width, _separationBar.Y);
            logEntryPosition.height = logEntryPosition.height - cur.y;
            _selectedLogEntryIndex = _logEntryArray.OnGUI(logEntryPosition, _selectedLogEntryIndex, _logEntries);
            DebugConsole.SetValue("logEntryPosition", logEntryPosition.ToString());
            DebugConsole.SetValue("y", _separationBar.Y.ToString());

            //GUI.DrawTexture(new Rect(100, 100, 100, 100), Texture2D.whiteTexture);

            //GUI.DrawTexture(logEntryPosition, Texture2D.whiteTexture);
            _separationBar.Render(position.width, cur.y, position.height);

            DebugConsole.SetValue("mouse", Event.current.mousePosition.ToString());
        }
    }
}