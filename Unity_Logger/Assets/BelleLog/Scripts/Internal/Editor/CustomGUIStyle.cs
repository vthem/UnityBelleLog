using UnityEngine;

namespace BelleLog.Internal.Editor
{
    public static class CustomGUIStyle
    {
        public static void SetConsoleFont(GUIStyle style)
        {
            style.font = (Font)Resources.Load("DroidSans");
            style.fontSize = 13;
        }
    }
}