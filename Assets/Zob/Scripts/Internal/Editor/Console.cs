using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class LogEntryRenderer : ITableLineRenderer
    {
        private ILogEntryContainer _container;
        private GUIStyles _styles;

        private Texture2D[] _levelTexture;

        public LogEntryRenderer(ILogEntryContainer container, GUIStyles styles)
        {
            _container = container;
            _styles = styles;

            _levelTexture = new Texture2D[]
            {
                new Texture2D(1, 1), // TRC
                new Texture2D(1, 1), // DBG
                new Texture2D(1, 1), // NFO
                new Texture2D(1, 1), // WRN
                new Texture2D(1, 1), // ERR
                new Texture2D(1, 1)  // FTL
            };
            _levelTexture[(int)LogLevel.Trace].SetPixel(0, 0, new Color32(0xB0, 0xB0, 0xB0, 0xff));
            _levelTexture[(int)LogLevel.Debug].SetPixel(0, 0, new Color32(0xFF, 0xFF, 0xFF, 0xff));
            _levelTexture[(int)LogLevel.Info].SetPixel(0, 0, new Color32(0x00, 0xFF, 0x00, 0xff));
            _levelTexture[(int)LogLevel.Warning].SetPixel(0, 0, new Color32(0xFF, 0x80, 0x00, 0xff));
            _levelTexture[(int)LogLevel.Error].SetPixel(0, 0, new Color32(0xFF, 0x00, 0x00, 0xff));
            _levelTexture[(int)LogLevel.Fatal].SetPixel(0, 0, new Color32(0x80, 0x00, 0xFF, 0xff));

            for (int i = 0; i  < _levelTexture.Length; ++i)
            {
                _levelTexture[i].Apply();
            }
        }

        public void OnGUI(Rect position, int index, int selectedIndex)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle s = index % 2 == 0 ? _styles.OddBackground : _styles.EvenBackground;
                s.Draw(position, false, false, selectedIndex == index, false);
            }

            var entry = _container[index];

            RenderLevelColor(position, entry);
            RenderTextLabel(position, entry);
        }

        private void RenderLevelColor(Rect position, LogEntry entry)
        {
            Rect colorPos = position;
            colorPos.x = 1;
            colorPos.width = 8;
            colorPos.y = colorPos.y + colorPos.height - 6f;
            colorPos.height = 2;
            GUI.DrawTexture(colorPos, _levelTexture[(int)entry.level]);
        }

        private void RenderTextLabel(Rect position, LogEntry entry)
        {
            position.x = 10;
            EditorGUI.LabelField(position, entry.content);
        }
    }

    internal class LogEntryCounter : ITableLineCount
    {
        private ILogEntryContainer _container;

        public LogEntryCounter(ILogEntryContainer container)
        {
            _container = container;
        }

        public int Count
        {
            get
            {
                return _container.Count;
            }
        }
    }

    internal class Console : EditorWindow
    {
        private readonly ConsoleLogHandler _logHandler = new ConsoleLogHandler();
        private ILogEntryContainer _logEntries;

        private int _selectedLogEntryIndex = -1;
        private GUISearchTab _searchFieldGUI;
        private GUILogEntryTable _logEntryTableGUI;
        private GUILogEntryContent _logEntryContentGUI;
        private GUILogEntryStackTrace _logEntryStackTraceGUI;
        private LogEntryCounter _logEntryCounter;

        private bool _initialized = false;
        private bool _onGUIInitialized = false;
        private bool _collapse;
        private bool _clearOnPlay;
        private bool _errorPause;
        private GUIStyles _guiStyles;
        private BottomMode _bottomMode = BottomMode.LogContent;
        private Texture2D _text;
        private Texture2D iconWarn;

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
            _searchFieldGUI = new GUISearchTab();

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
            _logEntries.Updated += NewLogEntryHandler;
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
            _logEntryContentGUI = new GUILogEntryContent(this, _guiStyles);
            _logEntryStackTraceGUI = new GUILogEntryStackTrace(this);
            _logEntryCounter = new LogEntryCounter(_logEntries);
            _logEntryTableGUI = new GUILogEntryTable(this, new LogEntryRenderer(_logEntries, _guiStyles));

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
                    _logHandler.ApplyFilters();
                }
            }
            GUILayout.EndHorizontal();

            _selectedLogEntryIndex = _searchFieldGUI.OnGUI(_selectedLogEntryIndex, _logEntries);

            _selectedLogEntryIndex = _logEntryTableGUI.OnGUI(_selectedLogEntryIndex, _logEntryCounter, _searchFieldGUI.HasUpdatedLogEntryIndex);

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
                    label = _logEntries[_selectedLogEntryIndex].content;
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