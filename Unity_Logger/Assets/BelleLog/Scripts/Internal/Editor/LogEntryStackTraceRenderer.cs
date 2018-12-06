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

        public LogEntryStackTraceRenderer()
        {
            _labelStyle = new GUIStyle(CustomGUIStyle.LabelStyle);
            _labelStyle.alignment = TextAnchor.MiddleLeft;
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
                var style = new GUIStyle(GUI.skin.box);
                style.margin = new RectOffset(0, 0, 1, 1);
                EditorGUILayout.BeginHorizontal(style);
                GUI.enabled = fileExist;

                int h = 18;
                if (GUILayout.Button(">", GUILayout.Height(h), GUILayout.Width(30)))
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
                        EditorGUILayout.LabelField(frame.className + "::" + frame.methodName + " (at " + shortFilename + " +" + frame.line + ")", _labelStyle, GUILayout.Height(h));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(shortFilename + " +" + frame.line, _labelStyle, GUILayout.Height(h));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(frame.className + "::" + frame.methodName, _labelStyle, GUILayout.Height(h));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}