using System;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class Console : EditorWindow
    {
        private readonly ConsoleLogHandler _logHandler = new ConsoleLogHandler();
        private ILogEntryContainer _logEntries;

        private int _selectedLogEntryIndex = -1;
        private SearchTab _searchFieldGUI;
        private LogEntryTable _logEntryTableGUI;
        private LogEntryContent _logEntryContentGUI;
        private LogEntryStackTrace _logEntryStackTraceGUI;
        private LogEntryCounter _logEntryCounter;

        private bool _initialized = false;
        private bool _onGUIInitialized = false;
        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private GUIStyles _guiStyles;
        private BottomMode _bottomMode = BottomMode.LogContent;
        private Texture2D _text;

        private ILogFilter[] _logLevelFilters;
        private string[] _logFilterToggleLabel;

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

            _text = new Texture2D(1, 1);
            _text.SetPixel(0, 0, Color.magenta);
            _text.Apply();

            _logFilterToggleLabel = new string[]
            {
                "TRC [{0}]",
                "DBG [{0}]",
                "NFO [{0}]",
                "WRN [{0}]",
                "ERR [{0}]",
                "FTL [{0}]"
            };
            _logLevelFilters = new PredicateLogFilter[]
            {
                new PredicateLogFilter((entry) => entry.level == LogLevel.Trace),
                new PredicateLogFilter((entry) => entry.level == LogLevel.Debug),
                new PredicateLogFilter((entry) => entry.level == LogLevel.Info),
                new PredicateLogFilter((entry) => entry.level == LogLevel.Warning),
                new PredicateLogFilter((entry) => entry.level == LogLevel.Error),
                new PredicateLogFilter((entry) => entry.level == LogLevel.Fatal),
            };
            for (int i = 0; i < _logLevelFilters.Length; ++i)
            {
                _logLevelFilters[i].Enable = false;
                _logHandler.AddFilter(_logLevelFilters[i]);
            }
            _logEntries = _logHandler;
            LogSystem.AddHandler(_logHandler);
            _selectedLogEntryIndex = -1;

            Repaint();
        }

        private void NewLogEntryHandler(ILogEntryContainer logEntries, LogEntry logEntry)
        {
            Repaint();
            _logEntryTableGUI.UpdateAutoScrolling(_logEntryCounter);
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
            _logEntryContentGUI = new LogEntryContent(this, _guiStyles);
            _logEntryStackTraceGUI = new LogEntryStackTrace(this);
            _logEntryCounter = new LogEntryCounter(_logEntries);
            _logEntryTableGUI = new LogEntryTable(this, new LogEntryRenderer(_logEntries, _guiStyles));
            _searchFieldGUI = new SearchTab(this);

            _logEntries.Updated += NewLogEntryHandler;
        }

        protected void OnGUIWarningNotProperlyInitialized()
        {
            if (!_initialized)
            {
                EditorGUILayout.LabelField("Not properly initialized");
                return;
            }
        }

        protected void OnGUIToolbar()
        {
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));
            if (GUILayout.Button("Clear", new GUIStyle("ToolbarButton")))
            {
                _selectedLogEntryIndex = -1;
                _logEntries.Clear();
            }

            EditorGUILayout.Space();

            GUI.enabled = false;
            _collapse = GUILayout.Toggle(_collapse, "Collapse", new GUIStyle("ToolbarButton"));
            _clearOnPlay = GUILayout.Toggle(_clearOnPlay, "Clear on Play", new GUIStyle("ToolbarButton"));
            _errorPause = GUILayout.Toggle(_errorPause, "Error Pause", new GUIStyle("ToolbarButton"));
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            for (int i = 0; i < _logLevelFilters.Length; ++i)
            {
                bool prevState = _logLevelFilters[i].Enable;
                _logLevelFilters[i].Enable = !GUILayout.Toggle(
                    !_logLevelFilters[i].Enable,
                    string.Format(_logFilterToggleLabel[i], _logEntries.CountByLevel((LogLevel)i)),
                    new GUIStyle("ToolbarButton")
                );

                if (prevState != _logLevelFilters[i].Enable)
                {
                    _selectedLogEntryIndex = -1;
                    _logHandler.ApplyFilters();
                    Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }

        protected void OnGUISearchBar()
        {
            _selectedLogEntryIndex = _searchFieldGUI.OnGUI(_selectedLogEntryIndex, _logEntries);
        }

        protected void OnGUILogEntryTable()
        {
            _selectedLogEntryIndex = _logEntryTableGUI.OnGUI(_selectedLogEntryIndex, _logEntryCounter, _searchFieldGUI.HasUpdatedLogEntryIndex);
        }

        protected void OnGUILogEntryInformation()
        {
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

            bool logEntryUpdated = _logEntryTableGUI.HasUpdatedLogEntryIndex || _searchFieldGUI.HasUpdatedLogEntryIndex;
            // drawing stacktrace or content modifies the GUILayout. yet it can't be modified before a new layout event
            if (_selectedLogEntryIndex != -1 && !logEntryUpdated)
            {
                if (_bottomMode == BottomMode.LogContent)
                {
                    string label = string.Empty;
                    label = _logEntries[_selectedLogEntryIndex].content;
                    _logEntryContentGUI.OnGUI(label);
                }
                else if (_bottomMode == BottomMode.StackTrace)
                {
                    LogEntry logEntry;
                    logEntry = _logEntries[_selectedLogEntryIndex];
                    _logEntryStackTraceGUI.OnGUI(logEntry);
                }
            }
        }

        protected void OnGUI()
        {
       //     Debug.Log("type=" + Event.current.type
       //+ " code=" + Event.current.keyCode
       //+ " shift=" + Event.current.shift
       //+ " editing=" + EditorGUIUtility.editingTextField);
            OnGUIInitialize();
            OnGUIWarningNotProperlyInitialized();
            OnGUIToolbar();
            OnGUISearchBar();
            OnGUILogEntryTable();
            OnGUILogEntryInformation();
        }
    }
}