using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
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
            _instance.InitializeOnce();
            _instance.hideFlags = HideFlags.HideAndDontSave;
        }

        protected void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            InitializeOnce();
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
        }

        protected void OnGUI()
        {
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

            if (GUILayout.Button("Access Unity Log"))
            {
                var assembly = Assembly.Load("UnityEditor.dll");
                Type customerType = assembly.GetType("UnityEditorInternal.LogEntries");
                MethodInfo staticMethodInfo = customerType.GetMethod("GetCount");
                int returnValue = Convert.ToInt32(staticMethodInfo.Invoke(null, null));
                Debug.Log("count=" + returnValue);
            }
        }

        private void LoadLines()
        {
            if (_lines == null)
            {
                _lines = File.ReadAllLines(Path.Combine(Application.dataPath, "sample.txt"));
                Debug.Log("loaded lines length=" + _lines.Length);
            }
        }

        private string PickRandomLine()
        {
            if (_lines == null)
            {
                return "no line loaded";
            }
            var rand = UnityEngine.Random.Range(0, _lines.Length);
            Debug.Log("length=" + _lines.Length + " rand=" + rand);

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