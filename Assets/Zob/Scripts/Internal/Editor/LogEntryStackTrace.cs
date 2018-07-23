using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class LogEntryStackTrace
    {
        private Vector2 _scrollValue;
        private EditorWindow _parent;

        public LogEntryStackTrace(EditorWindow parent)
        {
            _parent = parent;
        }

        public void OnGUI(LogEntry logEntry)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, false, false);
            var frames = logEntry.stackTrace.GetFrames();
            for (int i = 0; i < frames.Length; ++i)
            {
                var frame = frames[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(">", GUILayout.Width(30)))
                {

                }
                var style = EditorStyles.label;
                style.alignment = TextAnchor.LowerLeft;
                EditorGUILayout.LabelField(frame.GetMethod().ReflectedType + "::" + frame.GetMethod().Name + " (at " + frame.GetFileName() + " +" + frame.GetFileLineNumber() + ")", style, GUILayout.Height(18));
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
    }
}