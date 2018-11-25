﻿using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    internal class LogEntryContentRenderer
    {
        private Vector2 _scrollValue;
        private EditorWindow _parent;
        private readonly GUIStyle _selectableLabel;

        public LogEntryContentRenderer(EditorWindow parent)
        {
            _parent = parent;
            _selectableLabel  = new GUIStyle("CN Message");
            CustomGUIStyle.SetConsoleFont(_selectableLabel);
        }

        public void OnGUI(string content)
        {
            _scrollValue = GUILayout.BeginScrollView(_scrollValue, false, false);
            var height = _selectableLabel.CalcHeight(new GUIContent(content), _parent.position.width);
            EditorGUILayout.SelectableLabel(
                content,
                _selectableLabel,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.Height(height));
            GUILayout.EndScrollView();
        }
    }
}