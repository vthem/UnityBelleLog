using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class SeparationBarGUI
    {
        private Texture2D _texture;

        public float Y { get; private set; }
        public float YPercent { get; private set; }

        public SeparationBarGUI(float yPercent)
        {
            _texture = new Texture2D(1, 1);
            _texture.SetPixel(0, 0, Color.black);
            _texture.Apply();
            YPercent = yPercent;
        }

        public void Render(float width, float parentHeight)
        {
            Y = parentHeight * YPercent;
            Rect position = new Rect(0, Y, width, 1);
            GUI.DrawTexture(position, _texture);

            Rect colliderRect = position;
            colliderRect.height = 5;

            if (Mou)
            //if (Event.current.type == EventType.mouseDown)
        }
    }
}