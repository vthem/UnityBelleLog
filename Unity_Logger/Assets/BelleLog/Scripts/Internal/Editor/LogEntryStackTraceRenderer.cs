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
        private GUIStyle _boxStyle;

        public LogEntryStackTraceRenderer()
        {
            _labelStyle = new GUIStyle("CN Message");
            _labelStyle.alignment = TextAnchor.LowerLeft;
            _boxStyle = new GUIStyle("CN Box");
            CustomGUIStyle.SetConsoleFont(_labelStyle);
        }

        public void OnGUI(LogEntry logEntry)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, _boxStyle);
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
                if (GUILayout.Button(">", GUILayout.Width(30)))
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
                        EditorGUILayout.LabelField(frame.className + "::" + frame.methodName + " (at " + shortFilename + " +" + frame.line + ")", _labelStyle, GUILayout.Height(18));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(shortFilename + " +" + frame.line, _labelStyle, GUILayout.Height(18));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(frame.className + "::" + frame.methodName, _labelStyle, GUILayout.Height(18));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}