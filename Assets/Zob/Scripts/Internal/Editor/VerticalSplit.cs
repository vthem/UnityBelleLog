using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class VerticalSplit
    {
        float _height = -1;
        Rect _colliderRect;
        bool _mouseDown;
        float _mouseY;
        float _heightStart;
        EditorWindow _window;
        float _x;

        public VerticalSplit(EditorWindow window, float x)
        {
            _window = window;
            _x = x;
        }

        public void OnGUI()
        {
            if (_height == -1)
            {
                _height = _window.position.height * 0.3f;
            }

            var second = GUILayoutUtility.GetRect(0, _height);

            if (Event.current.type == EventType.Repaint)
            {
                second.width = 10;
                second.x = _x;
                GUI.DrawTexture(second, Texture2D.whiteTexture);

                _colliderRect.x = 0;
                _colliderRect.y = second.y + second.height;
                _colliderRect.width = _window.position.width;
                _colliderRect.height = 5;
                EditorGUIUtility.AddCursorRect(_colliderRect, MouseCursor.ResizeVertical);
            }


            if (Event.current.type == EventType.mouseDown && _colliderRect.Contains(Event.current.mousePosition))
            {
                _mouseY = Event.current.mousePosition.y;
                _heightStart = second.height;
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