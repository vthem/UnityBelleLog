#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogEntryStackTraceRenderer
    {
        private Vector2 _scrollValue;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private int _height = 18;

        public LogEntryStackTraceRenderer()
        {
            _labelStyle = new GUIStyle(CustomGUIStyle.LabelStyle);
            _labelStyle.alignment = TextAnchor.UpperLeft;
            _labelStyle.contentOffset = new Vector2(0, 1);
            _labelStyle.fixedHeight = _height;

            _buttonStyle = new GUIStyle(EditorStyles.miniButton);

            CustomGUIStyle.SetConsoleFont(_labelStyle);
        }

        public void OnGUI(LogEntry logEntry)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, CustomGUIStyle.BoxStyle);
            if (logEntry.stackTrace == null)
            {
                EditorGUILayout.LabelField("no stacktrace available", _labelStyle);
                GUILayout.EndScrollView();
                return;
            }
            if (logEntry.stackTrace.Length == 0)
            {
                EditorGUILayout.LabelField("no frame available", _labelStyle);
                GUILayout.EndScrollView();
                return;
            }

            for (int i = 0; i < logEntry.stackTrace.Length; ++i)
            {
                var frame = logEntry.stackTrace[i];
                var fileExist = File.Exists(frame.fileName);
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = fileExist;

                if (GUILayout.Button(">", _buttonStyle, GUILayout.Width(18)))
                {
                    InternalEditorUtility.OpenFileAtLineExternal(frame.fileName, frame.line);
                }
                GUI.enabled = true;

                if (fileExist)
                {
                    int assetPos = frame.fileName.LastIndexOf("Assets");
                    string shortFilename = frame.fileName;
                    if (assetPos > 0)
                    {
                        shortFilename = shortFilename.Substring(assetPos);
                    }

                    if (!string.IsNullOrEmpty(frame.className))
                    {
                        EditorGUILayout.LabelField(frame.className + "::" + frame.methodName + " (at " + shortFilename + " +" + frame.line + ")", _labelStyle, GUILayout.Height(_height));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(shortFilename + " +" + frame.line, _labelStyle, GUILayout.Height(_height));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(frame.className + "::" + frame.methodName, _labelStyle, GUILayout.Height(_height));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}
#endif