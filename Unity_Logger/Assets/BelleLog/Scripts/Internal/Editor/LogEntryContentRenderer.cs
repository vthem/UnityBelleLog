using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogEntryContentRenderer
    {
        private Vector2 _scrollValue;
        private EditorWindow _parent;
        private readonly GUIStyle _selectableLabelStyle;

        public LogEntryContentRenderer(EditorWindow parent)
        {
            _parent = parent;
            _selectableLabelStyle = new GUIStyle(CustomGUIStyle.LabelStyle);
            CustomGUIStyle.SetConsoleFont(_selectableLabelStyle);
        }

        public void OnGUI(string content)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, CustomGUIStyle.BoxStyle);
            var height = _selectableLabelStyle.CalcHeight(new GUIContent(content), _parent.position.width);
            EditorGUILayout.SelectableLabel(
                content,
                _selectableLabelStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.Height(height));
            GUILayout.EndScrollView();
        }
    }
}