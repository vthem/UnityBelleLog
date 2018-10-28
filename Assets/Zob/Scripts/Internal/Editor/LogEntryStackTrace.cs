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
            var frames = logEntry.stackTrace.GetFrames();
            if (frames.Length == 0)
            {
                EditorGUILayout.LabelField("no frame available");
                return;
            }
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, false, false);
            for (int i = 0; i < frames.Length; ++i)
            {
                var frame = frames[i];
                var fileExist = File.Exists(frame.GetFileName());
                EditorGUILayout.BeginHorizontal();
                GUI.enabled = fileExist;
                if (GUILayout.Button(">", GUILayout.Width(30)))
                {
                    InternalEditorUtility.OpenFileAtLineExternal(frame.GetFileName(), frame.GetFileLineNumber());
                }
                GUI.enabled = true;
                var style = EditorStyles.label;
                style.alignment = TextAnchor.LowerLeft;
                if (fileExist)
                {
                    EditorGUILayout.LabelField(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name + " (at " + frame.GetFileName() + " +" + frame.GetFileLineNumber() + ")", style, GUILayout.Height(18));
                }
                else
                {
                    EditorGUILayout.LabelField(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name, style, GUILayout.Height(18));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}