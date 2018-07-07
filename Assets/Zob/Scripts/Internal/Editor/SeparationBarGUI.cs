using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class SeparationBarGUI
    {
        private Texture2D _texture;

        public float Y { get; private set; }
        public float YPercent { get; private set; }

        private bool _mouseDown = false;
        private EditorWindow _window;

        public SeparationBarGUI(float yPercent, EditorWindow parent)
        {
            _window = parent;
            _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, Color.black);
            _texture.Apply();
            YPercent = yPercent;
        }

        public void Render(float width, float minY, float maxY)
        {
            if (Event.current.type != EventType.Layout)
            {
                Y = Mathf.Lerp(minY, maxY, YPercent);
                var drawRect = new Rect(0, Y, width, 1);
                GUI.DrawTexture(drawRect, _texture);

                Rect colliderRect = drawRect;
                colliderRect.height = 5;

                EditorGUIUtility.AddCursorRect(colliderRect, MouseCursor.ResizeVertical);

                if (Event.current.type == EventType.mouseDown && colliderRect.Contains(Event.current.mousePosition))
                {
                    _mouseDown = true;
                }
                if (_mouseDown && Event.current.type == EventType.MouseUp)
                {
                    _mouseDown = false;
                }
                if (Event.current.type == EventType.MouseDrag && _mouseDown)
                {
                    YPercent = Mathf.InverseLerp(minY, maxY, Event.current.mousePosition.y);
                    _window.Repaint();
                }
            }
        }
    }
}