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
            style.font = (Font)Resources.Load("Consola");
            style.fontSize = 13;
        }

        private static Color32[] _logLevelColors = new Color32[]
        {
            new Color32(181, 230, 29, 0xff),
            new Color32(153, 217, 234, 0xff),
            new Color32(255, 255, 255, 0xff),
            new Color32(255, 201, 14, 0xff),
            new Color32(237, 28, 36, 0xff),
            new Color32(206, 0, 190, 0xff),
        };

        private static Texture2D[] _logLevelTextures = null;
        public static Texture2D TextureFromLogLevel(LogLevel level)
        {
            if (_logLevelTextures == null)
            {
                _logLevelTextures = new Texture2D[_logLevelColors.Length];
                for (int i = 0; i < _logLevelColors.Length; ++i)
                {
                    _logLevelTextures[i] = new Texture2D(1, 1);
                    _logLevelTextures[i].SetPixel(0, 0, _logLevelColors[i]);
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
                _logLevelDarkTextures = new Texture2D[_logLevelColors.Length];
                for (int i = 0; i < _logLevelColors.Length; ++i)
                {
                    _logLevelDarkTextures[i] = new Texture2D(1, 1);
                    Color c = _logLevelColors[i];
                    c.r -= 5;
                    c.g -= 5;
                    c.b -= 5;
                    _logLevelDarkTextures[i].SetPixel(0, 0, c);
                    _logLevelDarkTextures[i].Apply();
                }
            }
            return _logLevelDarkTextures[(int)level];
        }
    }
}