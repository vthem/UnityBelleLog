using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class GUIVerticalSplit
    {
        public Rect Position { get { return _position; } set { _position = value; } }

        private Rect _colliderRect;
        private Rect _position;
        private bool _mouseDown;
        private float _mouseY;
        private float _heightStart;
        private float _height;
        private EditorWindow _window;

        public GUIVerticalSplit(EditorWindow window)
        {
            _window = window;
            _height = -1;
        }

        public void OnGUI()
        {
            if (_height == -1)
            {
                _height = _window.position.height * 0.3f;
            }

            _position = GUILayoutUtility.GetRect(0, _height);

            if (Event.current.type == EventType.Repaint)
            {
                _colliderRect.x = 0;
                _colliderRect.y = _position.y + _position.height;
                _colliderRect.width = _window.position.width;
                _colliderRect.height = 2;
                EditorGUIUtility.AddCursorRect(_colliderRect, MouseCursor.ResizeVertical);
            }

            if (Event.current.type == EventType.MouseDown && _colliderRect.Contains(Event.current.mousePosition))
            {
                _mouseY = Event.current.mousePosition.y;
                _heightStart = _height;
                _mouseDown = true;
            }
            if (_mouseDown && Event.current.type == EventType.MouseUp)
            {
                _mouseDown = false;
            }
            if (Event.current.type == EventType.MouseDrag && _mouseDown)
            {
                var delta = _mouseY - Event.current.mousePosition.y;
                _height = _heightStart - delta;
                _window.Repaint();
            }
        }
    }
}