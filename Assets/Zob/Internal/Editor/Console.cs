using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{

    public class Console : EditorWindow
    {
        Vector2 scrollPos;
        int count = 0;
        int texSize = 10;
        Rect _rect;
        Console _window;
        float _scrollValue;

        float _rowHeight = 30f;

        Texture2D _greyTex;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var windowRef = (Console)EditorWindow.GetWindow(typeof(Console));
            windowRef._window = windowRef;
            windowRef._window.titleContent = new GUIContent("ZobConsole");
            windowRef._window.Show();
            //windowRef.wantsMouseMove = true;
            windowRef._greyTex = new Texture2D(1, 1);
            windowRef._greyTex.SetPixel(0, 0, Color.cyan);
            windowRef._greyTex.Apply();
            Debug.Log("open zop console window");
        }

        void OnGUI()
        {
            //EditorGUILayout.BeginHorizontal();
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            //GUILayout.BeginArea(new Rect(0, 0, texSize, texSize), Texture2D.whiteTexture);
            //GUILayout.EndArea();
            //for (int i = 0; i < count; ++i)
            //{
            //    GUILayout.Label(i.ToString());
            //}
            //EditorGUILayout.EndScrollView();
            //EditorGUILayout.EndHorizontal();
            //if (GUILayout.Button("+10"))
            //    count += 10;
            //if (GUILayout.Button("+10 tex"))
            //    texSize += 10;
            //EditorGUILayout.LabelField(scrollPos.ToString());
            //_rect = EditorGUILayout.RectField(_rect);
            //GUI.Label(_rect, "test");
            //EditorGUILayout.RectField("position", _window.position);
            //EditorGUILayout.Vector2Field("mouse", Event.current.mousePosition);
            //EditorGUILayout.Vector2Field("delta", Event.current.delta);
            //for (int i = 0; i < 10; ++i)
            //{
            //    GUI.DrawTexture(new Rect(0, i * _rowHeight, _window.position.width - GUI.skin.verticalScrollbar.fixedWidth, _rowHeight), Texture2D.whiteTexture);
            //}
            GUI.DrawTexture(new Rect(0, 50, _window.position.width, 1), _greyTex);
            // EditorGUIUtility.AddCursorRect

            _scrollValue = GUI.VerticalScrollbar(
                new Rect(_window.position.width - GUI.skin.verticalScrollbar.fixedWidth, 0, GUI.skin.verticalScrollbar.fixedWidth, _window.position.height),
                _scrollValue, 1, 0, 10);

            //if (Event.current.type == EventType.MouseMove)
            //    Repaint();
            if (Event.current.type == EventType.ScrollWheel)
            {
                _scrollValue += Event.current.delta.y;
                Mathf.Clamp(_scrollValue, 0, 10);
                Repaint();
            }

        }
    }
}