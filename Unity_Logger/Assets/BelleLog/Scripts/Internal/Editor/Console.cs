using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using BelleLog.Internal.Editor.Filter;

namespace BelleLog.Internal.Editor
{
    internal class Console : EditorWindow
    {
        private readonly ConsoleLogHandler _logHandler = new ConsoleLogHandler();
        private ILogEntryContainer _logEntries;

        private int _selectedLogEntryIndex = -1;
        private SearchTabRenderer _searchFieldGUI;
        private TableRenderer _logEntryTableGUI;
        private LogEntryContentRenderer _logEntryContentGUI;
        private LogEntryStackTraceRenderer _logEntryStackTraceGUI;
        private LogEntryCounter _logEntryCounter;
        private LogEntryRenderer _logEntryRenderer;

        private bool _initialized = false;
        private bool _onGUIInitialized = false;
        private BottomMode _bottomMode = BottomMode.LogContent;

        private ILogFilter[] _logLevelFilters;
        private bool[] _enableLogLevelColors;
        private string[] _logFilterToggleLabel;
        private CollapseLogFilter _collapseFilter;

        private enum BottomMode
        {
            LogContent,
            StackTrace
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/BelleConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.Show();
        }

        public Console()
        {
        }

        void OnEnable()
        {
            Initialize();
        }

        void OnDisable()
        {
            _initialized = false;
            _onGUIInitialized = false;
            if (_logEntries != null)
            {
                _logEntries.Updated -= NewLogEntryHandler;
            }
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
            titleContent = new GUIContent("BelleConsole");

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

            _collapseFilter = new CollapseLogFilter();
            _logHandler.AddFilter(_collapseFilter);

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
            _enableLogLevelColors = new bool[_logLevelFilters.Length];
            for (int i = 0; i < _logLevelFilters.Length; ++i)
            {
                _logLevelFilters[i].Enable = false;
                _enableLogLevelColors[i] = false;
                _logHandler.AddFilter(_logLevelFilters[i]);
            }
            _enableLogLevelColors[(int)LogLevel.Warning] = true;
            _enableLogLevelColors[(int)LogLevel.Error] = true;
            _logEntries = _logHandler;
            LogSystem.AddHandler(_logHandler);
            _selectedLogEntryIndex = -1;

            // Initialize things that need to be initialize in OnGUI
            _logEntryContentGUI = new LogEntryContentRenderer(this);
            _logEntryStackTraceGUI = new LogEntryStackTraceRenderer();
            _logEntryCounter = new LogEntryCounter(_logEntries);
            _logEntryRenderer = new LogEntryRenderer(_logEntries, _collapseFilter);
            _logEntryRenderer.EnableLevelColors = _enableLogLevelColors;
            _logEntryTableGUI = new TableRenderer(this, _logEntryRenderer);
            _searchFieldGUI = new SearchTabRenderer(this);

            _logEntries.Updated += NewLogEntryHandler;

            UnityEditorInternal unityInternalLog = new UnityEditorInternal();
            unityInternalLog.AddInternalLog();
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
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
            {
                _selectedLogEntryIndex = -1;
                _logEntries.Clear();
            }

            EditorGUILayout.Space();

            var newCollapseState = GUILayout.Toggle(_collapseFilter.Enable, "Collapse", EditorStyles.toolbarButton);
            if (newCollapseState != _collapseFilter.Enable)
            {
                _collapseFilter.Enable = newCollapseState;
                _logHandler.ApplyFilters();
            }
            GUILayout.FlexibleSpace();

            for (int i = 0; i < _logLevelFilters.Length; ++i)
            {
                bool prevState = _logLevelFilters[i].Enable;
                Color normalColor = EditorStyles.toolbarButton.normal.textColor;
                Color onNormalColor = EditorStyles.toolbarButton.onNormal.textColor;
                if (_enableLogLevelColors[i])
                {
                    EditorStyles.toolbarButton.normal.textColor = _logEntryRenderer.GetLevelColor((LogLevel)i);
                    EditorStyles.toolbarButton.onNormal.textColor = _logEntryRenderer.GetLevelColor((LogLevel)i);
                }
                bool newState = !GUILayout.Toggle(
                    !_logLevelFilters[i].Enable,
                    string.Format(_logFilterToggleLabel[i], _logEntries.CountByLevel((LogLevel)i)),
                    EditorStyles.toolbarButton
                );
                EditorStyles.toolbarButton.normal.textColor = normalColor;
                EditorStyles.toolbarButton.onNormal.textColor = onNormalColor;

                if (prevState != newState)
                {
                    if (Event.current.button == 0)
                    {
                        _selectedLogEntryIndex = -1;
                        _logLevelFilters[i].Enable = newState;
                        _logHandler.ApplyFilters();
                        Repaint();
                    }
                    else
                    {
                        _enableLogLevelColors[i] = !_enableLogLevelColors[i];
                        _logEntryRenderer.EnableLevelColors = _enableLogLevelColors;
                    }
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

            if (_logEntryTableGUI.HasDoubleClickedEntry)
            {
                OpenIDE(_logEntries[_selectedLogEntryIndex]);
            }
        }

        protected void OnGUILogEntryInformation()
        {
            GUILayout.BeginHorizontal(new GUIStyle("Toolbar"));

            if (GUILayout.Toggle(_bottomMode == BottomMode.LogContent, "Content", EditorStyles.toolbarButton))
            {
                _bottomMode = BottomMode.LogContent;
            }
            if (GUILayout.Toggle(_bottomMode == BottomMode.StackTrace, "Stack trace", EditorStyles.toolbarButton))
            {
                _bottomMode = BottomMode.StackTrace;
            }
            GUILayout.EndHorizontal();

            bool logEntryUpdated = _logEntryTableGUI.HasUpdatedSelectedEntry || _searchFieldGUI.HasUpdatedLogEntryIndex;
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

        Event fatalEvent;
        System.Exception fatalException;
        bool fatalIssue = false;

        protected void OnGUI()
        {
            if (fatalIssue)
            {
                EditorGUILayout.LabelField("eventType=" + fatalEvent.type);
                EditorGUILayout.LabelField("exeption message=" + fatalException.Message);
            }
            try
            {
                OnGUIInitialize();
                OnGUIWarningNotProperlyInitialized();
                OnGUIToolbar();
                OnGUISearchBar();
                OnGUILogEntryTable();
                OnGUILogEntryInformation();
            }
            catch (System.Exception ex)
            {
                fatalIssue = true;
                fatalException = ex;
                fatalEvent = Event.current;
            }
        }

        protected void OpenIDE(LogEntry entry)
        {
            for (int i = 0; i < entry.stackTrace.Length; ++i)
            {
                if (File.Exists(entry.stackTrace[i].fileName))
                {
                    InternalEditorUtility.OpenFileAtLineExternal(entry.stackTrace[i].fileName, entry.stackTrace[i].line);
                }
            }
        }
    }
}