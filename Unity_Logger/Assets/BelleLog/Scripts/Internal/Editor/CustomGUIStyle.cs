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

        private static Texture2D[] _logLevelTextures = null;
        public static Texture2D TextureFromLogLevel(LogLevel level)
        {
            if (_logLevelTextures == null)
            {
                _logLevelTextures = new Texture2D[LogLevelColors.Length];
                for (int i = 0; i < LogLevelColors.Length; ++i)
                {
                    _logLevelTextures[i] = new Texture2D(1, 1);
                    _logLevelTextures[i].SetPixel(0, 0, LogLevelColors[i]);
                    _logLevelTextures[i].Apply();
                }
            }
            return _logLevelTextures[(int)level];
        }

        private static Texture2D[] _logLevelDarkTextures = null;
        public static Texture2D DarkTextureFromLogLevel(LogLevel level)
        {
            if (_logLevelDarkTextures == null)
            {
                _logLevelDarkTextures = new Texture2D[LogLevelColors.Length];
                for (int i = 0; i < LogLevelColors.Length; ++i)
                {
                    _logLevelDarkTextures[i] = new Texture2D(1, 1);
                    Color32 c = LogLevelColors[i];
                    c.r = (byte)Mathf.Clamp(c.r - 10, 0, 255);
                    c.g = (byte)Mathf.Clamp(c.g - 10, 0, 255); ;
                    c.b = (byte)Mathf.Clamp(c.b - 10, 0, 255); ;
                    _logLevelDarkTextures[i].SetPixel(0, 0, c);
                    _logLevelDarkTextures[i].Apply();
                }
            }
            return _logLevelDarkTextures[(int)level];
        }
    }
}