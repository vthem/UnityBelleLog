using UnityEngine;

namespace BelleLog.Internal.Editor
{
    public static class CustomGUIStyle
    {
        private static GUIStyle _boxStyle;
        public static GUIStyle BoxStyle
        {
            get
            {
                if (_boxStyle == null)
                {
                    _boxStyle = new GUIStyle("CN Box");
                }
                return _boxStyle;
            }
        }

        private static GUIStyle _labelStyle;
        public static GUIStyle LabelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle("CN Message");
                }
                return _labelStyle;
            }
        }

        private static GUIStyle _toolbarStyle;
        public static GUIStyle ToolbarStyle
        {
            get
            {
                if (_toolbarStyle == null)
                {
                    _toolbarStyle = new GUIStyle("Toolbar");
                }
                return _toolbarStyle;
            }
        }

        public static void SetConsoleFont(GUIStyle style)
        {
            style.font = (Font)Resources.Load("DroidSans");
            style.fontSize = 13;
        }
    }
}