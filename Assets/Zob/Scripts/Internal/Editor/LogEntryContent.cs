using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class LogEntryContent
    {
        private Vector2 _scrollValue;
        private EditorWindow _parent;
        private GUIStyles _styles;

        public LogEntryContent(EditorWindow parent, GUIStyles styles)
        {
            _parent = parent;
            _styles = styles;
        }

        public void OnGUI(string content)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, false, false);
            var height = _styles.SelectableLabel.CalcHeight(new GUIContent(content), _parent.position.width);
            EditorGUILayout.SelectableLabel(
                content,
                _styles.SelectableLabel,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.Height(height));
            GUILayout.EndScrollView();
        }
    }
}