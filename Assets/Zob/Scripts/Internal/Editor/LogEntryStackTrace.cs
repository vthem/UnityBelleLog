using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class LogEntryStackTrace
    {
        private Vector2 _scrollValue;

        public void OnGUI(LogEntry logEntry)
        {
            if (logEntry.stackTrace == null)
            {
                EditorGUILayout.LabelField("no stacktrace available");
                return;
            }
            if (logEntry.stackTrace.Length == 0)
            {
                EditorGUILayout.LabelField("no frame available");
                return;
            }
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, false, false);
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
                var style = EditorStyles.label;
                style.alignment = TextAnchor.LowerLeft;
                if (fileExist)
                {
                    if (!string.IsNullOrEmpty(frame.className))
                    {
                        EditorGUILayout.LabelField(frame.className + "::" + frame.methodName + " (at " + frame.fileName + " +" + frame.line + ")", style, GUILayout.Height(18));
                    }
                    else
                    {
                        EditorGUILayout.LabelField(frame.fileName + " +" + frame.line, style, GUILayout.Height(18));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(frame.className + "::" + frame.methodName, style, GUILayout.Height(18));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}