﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    public class DebugConsole : EditorWindow
    {
        private Dictionary<string, string> _keyValue = new Dictionary<string, string>();
        private static DebugConsole _instance;
        private string[] _lines;
        private int _current = 0;
        private Logger _logger;
        private bool _initialized = false;
        private Texture2D _texture;
        private string _text;
        private GUIStyle _labelStyle;


        public static void SetValue(string key, string value)
        {
            if (_instance != null)
            {
                _instance._keyValue[key] = value;
                _instance.Repaint();
            }
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/DebugConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            _instance = (DebugConsole)EditorWindow.GetWindow(typeof(DebugConsole));
            _instance.Show();
            _instance.hideFlags = HideFlags.HideAndDontSave;

        }

        protected void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }
        }

        protected void InitializeOnce()
        {
            if (_initialized)
            {
                return;
            }

            _logger = new Logger();
            _instance._keyValue["start"] = DateTime.Now.ToShortDateString();

            _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, Color.magenta);
            _texture.Apply();
            _texture.hideFlags = HideFlags.HideAndDontSave;
            _initialized = true;

            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            _labelStyle.font = (Font)Resources.Load("Inconsolata-Regular");
            _labelStyle.fontSize = 12;
        }

        protected void OnGUI()
        {
            InitializeOnce();

            foreach (var kv in _keyValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(kv.Key);
                EditorGUILayout.SelectableLabel(kv.Value);
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("-- reset key value --"))
            {
                _keyValue.Clear();
            }

            if (GUILayout.Button("-- add 1x --"))
            {
                LoadLines();
                var line = PickRandomLine();
                AddRandomLog(line);
            }
            if (GUILayout.Button("-- add 1x (same)--"))
            {
                LoadLines();
                AddRandomLog(_lines[0]);
            }
            if (GUILayout.Button("-- add 50x --"))
            {
                LoadLines();
                for (int i = 0; i < 50; ++i)
                {
                    var line = PickRandomLine();
                    AddRandomLog(line);
                }
            }
            if (GUILayout.Button("-- add 50x (index) --"))
            {
                for (int i = 0; i < 50; ++i)
                {
                    AddRandomLog(i.ToString());
                }
            }

            if (GUILayout.Button("-- add unity warning --"))
            {
                Debug.LogWarning("warning log from unity");
            }


            if (GUILayout.Button("-- add unity error --"))
            {
                Debug.LogError("error log from unity");
            }

            if (GUILayout.Button("-- add unity exception --"))
            {
                throw new System.Exception("ahah!");
            }

            if (GUILayout.Button("-- cr --"))
            {
                throw new System.Exception("ahah!");
            }

            if (GUILayout.Button("-- add 5x --"))
            {
                for (int i = 0; i < 5; ++i)
                {
                    Debug.Log(_text);
                }
            }

            _text = EditorGUILayout.TextArea(_text, _labelStyle);
        }

        private void LoadLines()
        {
            if (_lines == null)
            {
                _lines = File.ReadAllLines(Path.Combine(Application.dataPath, "sample.txt"));
            }
        }

        private string PickRandomLine()
        {
            if (_lines == null)
            {
                return "no line loaded";
            }
            var rand = UnityEngine.Random.Range(0, _lines.Length);

            return _lines[rand];
        }

        private void AddRandomLog(string line)
        {
            var randomLevel = (LogLevel)UnityEngine.Random.Range((int)LogLevel.Trace, (int)LogLevel.Fatal);
            switch (randomLevel)
            {
                case LogLevel.Trace:
                _logger.Trace();
                break;
                case LogLevel.Debug:
                _logger.Debug(line);
                break;
                case LogLevel.Info:
                _logger.Info(line);
                break;
                case LogLevel.Warning:
                _logger.Warning(line);
                break;
                case LogLevel.Error:
                _logger.Error(line);
                break;
                case LogLevel.Fatal:
                _logger.Fatal(line);
                break;
                default:
                throw new System.Exception("unsupported random log level");
            }
        }

        private void IncCurrent()
        {
            _current = (_current + 1) % _lines.Length;
        }
    }
}