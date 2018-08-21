using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class Console : EditorWindow
    {
        private readonly ConsoleLogHandler _logHandler = new ConsoleLogHandler();
        private ILogEntryContainer _logEntries;

        private int _selectedLogEntryIndex = -1;
        private GUISearchField _searchFieldGUI;
        private GUILogEntryTable _logEntryTableGUI;
        private GUILogEntryContent _logEntryContentGUI;
        private GUILogEntryStackTrace _logEntryStackTraceGUI;

        private bool _initialized = false;
        private bool _onGUIInitialized = false;
        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private bool _showFilter;
        private GUIStyles _guiStyles;
        private BottomMode _bottomMode = BottomMode.LogContent;
        private Texture2D _text;

        private enum BottomMode
        {
            LogContent,
            StackTrace
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.Show();
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
            _onGUIInitialized = false;
            _logEntries.Updated -= NewLogEntryHandler;
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            _onGUIInitialized = false;

            wantsMouseMove = true;
            titleContent = new GUIContent("ZobConsole");
            _searchFieldGUI = new GUISearchField();

            _text = new Texture2D(1, 1);
            _text.SetPixel(0, 0, Color.magenta);
            _text.Apply();

            _logEntries = _logHandler;
            LogSystem.AddHandler(_logHandler);
            _logEntries.Updated += NewLogEntryHandler;
            _selectedLogEntryIndex = -1;

            Repaint();
        }

        private void NewLogEntryHandler(ILogEntryContainer logEntries, LogEntry logEntry)
        {
            Repaint();
            _logEntryTableGUI.UpdateAutoScrolling(logEntries);
        }

        protected void OnGUIInitialize()
        {
            if (_onGUIInitialized)
            {
                return;
            }
            _onGUIInitialized = true;

            // Initialize things that need to be initialize in OnGUI
            _guiStyles = new GUIStyles();
            _logEntryContentGUI = new GUILogEntryContent(this, _guiStyles);
            _logEntryStackTraceGUI = new GUILogEntryStackTrace(this);
            _logEntryTableGUI = new GUILogEntryTable(this, _guiStyles);
        }

        protected void OnGUI()
        {
            OnGUIInitialize();

            if (!_initialized)
            {
                EditorGUILayout.LabelField("Not properly initialized");
                return;
            }
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
            {
                _selectedLogEntryIndex = -1;
                _logEntries.Clear();
            }

            EditorGUILayout.Space();

            _collapse = GUILayout.Toggle(_collapse, "Collapse", new GUIStyle("ToolbarButton"));
            _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
            _errorPause = GUILayout.Toggle(_errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
            GUILayout.FlexibleSpace();
            _showFilter = GUILayout.Toggle(_showFilter, "Show filter", new GUIStyle("ToolbarButton"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));
            _selectedLogEntryIndex = _searchFieldGUI.OnGUI(_selectedLogEntryIndex, _logEntries);
            GUILayout.Space(5f);
            var maxWidth = GUILayout.MaxWidth(20);
            if (GUILayout.Button("<", new GUIStyle("ToolbarButton"), maxWidth))
            {
            }
            if (GUILayout.Button(">", new GUIStyle("ToolbarButton"), maxWidth))
            {
            }
            GUILayout.EndHorizontal();

            _selectedLogEntryIndex = _logEntryTableGUI.OnGUI(_selectedLogEntryIndex, _logEntries);

            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            if (GUILayout.Toggle(_bottomMode == BottomMode.LogContent, "Content", new GUIStyle("ToolbarButton")))
            {
                _bottomMode = BottomMode.LogContent;
            }
            if (GUILayout.Toggle(_bottomMode == BottomMode.StackTrace, "Stack trace", new GUIStyle("ToolbarButton")))
            {
                _bottomMode = BottomMode.StackTrace;
            }
            GUILayout.EndHorizontal();

            if (_bottomMode == BottomMode.LogContent)
            {
                string label = string.Empty;
                if (_selectedLogEntryIndex != -1)
                {
                    label = _logEntries.Content(_selectedLogEntryIndex);
                }
                _logEntryContentGUI.OnGUI(label);
            }
            else if (_bottomMode == BottomMode.StackTrace)
            {
                LogEntry logEntry;
                if (_selectedLogEntryIndex != -1)
                {
                    logEntry = _logEntries[_selectedLogEntryIndex];
                    _logEntryStackTraceGUI.OnGUI(logEntry);
                }
            }
        }
    }
}