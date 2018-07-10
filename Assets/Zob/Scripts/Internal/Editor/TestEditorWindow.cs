using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public class TestEditorWindow : EditorWindow
    {

        private int value = 0;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/TestEditorWindow")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var instance = (TestEditorWindow)EditorWindow.GetWindow(typeof(TestEditorWindow));
            instance.Show();
        }

        public TestEditorWindow()
        {
            Debug.Log("constructor " + value);
        }

        private void Awake()
        {
            Debug.Log("Awake " + value);
        }

        protected void OnGUI()
        {
            if (Event.current != null)
            {
                Debug.Log(Event.current.type.ToString() + " v=" + value);
            }
            else
            {
                Debug.Log("~null v=" + value);
            }
            if (GUILayout.Button("click"))
            {
                value++;
            }



        }
    }
}