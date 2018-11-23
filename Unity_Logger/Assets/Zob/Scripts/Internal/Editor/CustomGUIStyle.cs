using UnityEngine;

namespace Zob.Internal.Editor
{
    public static class CustomGUIStyle
    {
        public static void SetConsoleFont(GUIStyle style)
        {
            style.font = (Font)Resources.Load("DroidSans");
            style.fontSize = 12;
        }
    }
}