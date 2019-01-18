#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogEntryContentRenderer
    {
        private Vector2 _scrollValue;
        private EditorWindow _parent;
        private readonly GUIStyle _selectableLabelStyle;
        private StringBuilder _contentStr = new StringBuilder();

        public LogEntryContentRenderer(EditorWindow parent)
        {
            _parent = parent;
            _selectableLabelStyle = new GUIStyle(CustomGUIStyle.LabelStyle);
            CustomGUIStyle.SetConsoleFont(_selectableLabelStyle);
        }

        public void OnGUI(LogEntry entry)
        {
            _contentStr.Length = 0;
            _contentStr.AppendLine("Level: " + entry.level);
            _contentStr.AppendLine("Time: " + entry.time.ToString());
            _contentStr.AppendLine("Frame: " + entry.frame);
            _contentStr.AppendLine("Domain: " + entry.domain);
            _contentStr.AppendLine("Content:");
            _contentStr.AppendLine(entry.content);
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, CustomGUIStyle.BoxStyle);
            var content = new GUIContent(_contentStr.ToString());
            var height = _selectableLabelStyle.CalcHeight(content, _parent.position.width);
            EditorGUILayout.SelectableLabel(
                _contentStr.ToString(),
                _selectableLabelStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.Height(height));
            GUILayout.EndScrollView();
        }
    }
}
#endif