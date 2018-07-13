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

        private Texture2D _text;

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

            _text = new Texture2D(1, 1);
            _text.SetPixel(0, 0, Color.magenta);
            _text.Apply();

            Repaint();

        }

        //protected void OnGUI()
        //{
        //    if (!_initialized)
        //    {
        //        EditorGUILayout.LabelField("Not properly initialized");
        //        return;
        //    }
        //    var toolbarPosition = new Rect(0, 0, position.width, TabHeight);
        //    GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

        //    if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
        //    {
        //    }

        //    EditorGUILayout.Space();
        //    EditorGUILayout.Space();
        //    EditorGUILayout.Space();

        //    _collapse = GUILayout.Toggle(_collapse, "Collapse", new GUIStyle("ToolbarButton"));
        //    _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
        //    _errorPause = GUILayout.Toggle(_errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
        //    GUILayout.FlexibleSpace();
        //    _showFilter = GUILayout.Toggle(_showFilter, "Show filter", new GUIStyle("ToolbarButton"));
        //    GUILayout.EndHorizontal();

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

        //    var cur = GUILayoutUtility.GetRect(0, 0, 0, 0);
        //    DebugConsole.SetValue("cur ", cur.ToString());
        //    var logEntryArrayHeight = _separationBar.Y - cur.y;
        //    DebugConsole.SetValue("height rq ", logEntryArrayHeight.ToString());
        //    var logEntryPosition = GUILayoutUtility.GetRect(position.width, logEntryArrayHeight);
        //    logEntryPosition.height = logEntryPosition.height - cur.y;
        //    DebugConsole.SetValue("logEntryPosition ", logEntryPosition.ToString());
        //    //_selectedLogEntryIndex = _logEntryArray.OnGUI(logEntryPosition, _selectedLogEntryIndex, _logEntries);
        //    //_separationBar.Render(position.width, cur.y, position.height);
        //    //GUI.DrawTexture(logEntryPosition, _text);

        //    var labelPos = GUILayoutUtility.GetRect(100, 100);
        //    DebugConsole.SetValue("labelPos ", labelPos.ToString());
        //    GUI.DrawTexture(labelPos, Texture2D.whiteTexture);
        //    //if (_selectedLogEntryIndex != -1)
        //    //{
        //    //    var style = new GUIStyle();
        //    //    style.alignment = TextAnchor.UpperRight;
        //    //    style.fontSize = 16;
        //    //    EditorGUILayout.SelectableLabel(_logEntries[_selectedLogEntryIndex].format, style);
        //    //}

        //    DebugConsole.SetValue("mouse ", Event.current.mousePosition.ToString());
        //}

        float _height = -1;
        Rect _colliderRect;
        bool _mouseDown;
        float _mouseY;
        float _heightStart;

        protected void OnGUI()
        {
            if (_height == -1)
            {
                _height = position.height * 0.6f;
            }

            var first = GUILayoutUtility.GetRect(0, 35);
            var second = GUILayoutUtility.GetRect(0, _height);
            var third = GUILayoutUtility.GetRect(0, 50);

            if (Event.current.type == EventType.Repaint)
            {
                first.width = 10;
                GUI.DrawTexture(first, Texture2D.whiteTexture);
                second.width = 10;
                second.x = 10;
                GUI.DrawTexture(second, Texture2D.whiteTexture);
                third.width = 10;
                third.x = 20;
                GUI.DrawTexture(third, Texture2D.whiteTexture);

                _colliderRect.x = 0;
                _colliderRect.y = third.y;
                _colliderRect.width = position.width;
                _colliderRect.height = 5;
                EditorGUIUtility.AddCursorRect(_colliderRect, MouseCursor.ResizeVertical);
            }


            if (Event.current.type == EventType.mouseDown && _colliderRect.Contains(Event.current.mousePosition))
            {
                _mouseY = Event.current.mousePosition.y;
                _heightStart = second.height;
                _mouseDown = true;
            }
            if (_mouseDown && Event.current.type == EventType.MouseUp)
            {
                _mouseDown = false;
            }
            if (Event.current.type == EventType.MouseDrag && _mouseDown)
            {
                var delta = _mouseY - Event.current.mousePosition.y;
                _height = _heightStart - delta;
                Repaint();
            }
        }
    }
}