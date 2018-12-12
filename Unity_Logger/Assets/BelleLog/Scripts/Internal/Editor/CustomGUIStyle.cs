using UnityEditor;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    public static class CustomGUIStyle
    {
        public static Color32 TraceColor = new Color32(0xA1, 0xCF, 0xDD, 0xff);
        public static Color32 DebugColor = new Color32(0xCD, 0xDC, 0x49, 0xff);
        public static Color32 InfoColor = new Color32(0x6F, 0x64, 0x56, 0xff);
        public static Color32 WarningColor = new Color32(0xFE, 0xE6, 0x59, 0xff);
        public static Color32 ErrorColor = new Color32(0xE9, 0x4B, 0x30, 0xff);
        public static Color32 FatalColor = new Color32(0xCB, 0x7E, 0x94, 0xff);

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
                    _labelStyle = EditorStyles.label;
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
            style.font = (Font)Resources.Load("Consola");
            style.fontSize = 13;
        }

        public static Color32[] LogLevelColors = new Color32[]
        {
            TraceColor,
            DebugColor,
            InfoColor,
            WarningColor,
            ErrorColor,
            FatalColor,
        };


    }
}