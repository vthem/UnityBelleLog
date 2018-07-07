using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class DebugConsole : EditorWindow
    {
        Dictionary<string, string> keyValue = new Dictionary<string, string>();

        static DebugConsole instance;

        public static void SetValue(string key, string value)
        {
            if (instance != null)
            {
                instance.keyValue[key] = value;
                instance.Repaint();
            }
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/DebugConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            instance = (DebugConsole)EditorWindow.GetWindow(typeof(DebugConsole));
            instance.Show();
            instance.keyValue["start"] = DateTime.Now.ToShortDateString();
        }

        protected void OnGUI()
        {
            foreach (var kv in keyValue)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(kv.Key);
                EditorGUILayout.SelectableLabel(kv.Value);
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("reset"))
            {
                keyValue.Clear();
            }
        }
    }
}