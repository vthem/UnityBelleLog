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
        private float _rowHeight = 30f;
        private bool _initialized = false;
        private ConsoleConfig _config;

        private Texture2D _row1;
        private Texture2D _row2;

        private List<LogEntry> _logEntries = new List<LogEntry>();

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.InitializeOnce();
            console.Show();
            Debug.Log("open zop console window");
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
            _row1.SetPixel(0, 0, new Color32(220, 220, 220, 255));
            _row1.Apply();

            _row2 = new Texture2D(1, 1);
            _row2.SetPixel(0, 0, new Color32(200, 200, 200, 255));
            _row2.Apply();

            for (int i = 0; i < 100; ++i)
            {
                var entry = new LogEntry();
                entry.args = null;
                entry.domain = "log-console";
                entry.format = "this is a log entry " + UnityEngine.Random.Range(0, 10000);
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
        }

        void RenderLogEntries(Rect position)
        {
            var scrollbarPosition = new Rect(position.x + position.width - GUI.skin.verticalScrollbar.fixedWidth, position.y, GUI.skin.verticalScrollbar.fixedWidth, position.height);
            _scrollValue = GUI.VerticalScrollbar(scrollbarPosition, _scrollValue, 1, 10f, 0f);
            for (int i = 0; i < 2; ++i)
            {
                Texture2D rowText = _row1;
                if (i % 2 == 0)
                {
                    rowText = _row2;
                }
                GUI.DrawTexture(new Rect(position.x, position.y + i * 20f, position.width - GUI.skin.verticalScrollbar.fixedWidth, 20f), rowText);
            }
        }
    }
}