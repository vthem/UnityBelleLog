using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{

    public class Console : EditorWindow
    {
        Vector2 scrollPos;
        int count = 0;
        int texSize = 10;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            Console window = (Console)EditorWindow.GetWindow(typeof(Console));
            window.titleContent = new GUIContent("ZobConsole");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginArea(new Rect(0, 0, texSize, texSize), Texture2D.whiteTexture);
            GUILayout.EndArea();
            for (int i = 0; i < count; ++i)
            {
                GUILayout.Label(i.ToString());
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("+10"))
                count += 10;
            if (GUILayout.Button("+10 tex"))
                texSize += 10;
            EditorGUILayout.LabelField(scrollPos.ToString());
        }
    }
}