#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using BelleLog.Internal.Editor.Filter;

namespace BelleLog.Internal.Editor
{
    internal class LogEntryRenderer : ITableLineRenderer
    {
        private ILogEntryContainer _container;
        private const string _defaultTimestampLabel = "0m00s000";
        private GUIStyle _labelStyle;
        private readonly Texture2D _oddBackgroundTexture;
        private readonly Texture2D _evenBackgroundTexture;
        private readonly GUIStyle _collapseCountStyle = new GUIStyle("CN CountBadge");

        private string[] _logLevelLabel = new string[]
        {
            "TRC",
            "DBG",
            "NFO",
            "WRN",
            "ERR",
            "FTL"
        };

        private Texture2D[] _logLevelTextures = null;
        private Texture2D[] _logLevelDarkTextures = null;
        private const int ColorWidth = 10;

        private CollapseLogFilter _collapseFilter;

        public bool[] EnableLevelColors { get; set; }

        public LogEntryRenderer(ILogEntryContainer container, CollapseLogFilter collapseFilter)
        {
            _container = container;
            _labelStyle = new GUIStyle(GUI.skin.label);
            _evenBackgroundTexture = new GUIStyle("CN EntryBackEven").normal.background;
            _oddBackgroundTexture = new GUIStyle("CN EntryBackodd").normal.background;
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            CustomGUIStyle.SetConsoleFont(_labelStyle);
            _collapseFilter = collapseFilter;
        }

        public void OnGUI(Rect position, int index, int selectedIndex)
        {
            if (!IndexValid(index))
            {
                return;
            }

            var entry = _container[index];
            Texture2D backgroundTexture = null;
            if (index % 2 == 0)
            {
                backgroundTexture = _oddBackgroundTexture;
            }
            else
            {
                backgroundTexture = _evenBackgroundTexture;
            }
            if (Event.current.type == EventType.Repaint)
            {
                GUI.DrawTexture(position, backgroundTexture);
            }

            var fontSize = _labelStyle.fontSize;
            var fontStyle = _labelStyle.fontStyle;
            bool entrySelected = index == selectedIndex;
            if (entrySelected)
            {
                _labelStyle.fontStyle = FontStyle.Bold;
            }
            position = RenderColor(position, entry, index);
            if (_collapseFilter.Enable)
            {
                position = RenderCollapseCount(position, index);
            }
            position = RenderLogLevel(position, entry);
            position = RenderTimestampLabel(position, entry);
            position = RenderFrameCount(position, entry);
            position = RenderTextLabel(position, entry);
            _labelStyle.fontStyle = fontStyle;
        }

        private bool IndexValid(int index)
        {
            return index >= 0 && index < _container.Count;
        }

        private Rect RenderCollapseCount(Rect position, int index)
        {
            Rect lpos = position;

            var count = _collapseFilter.CollapseCount(index);
            var content = new GUIContent(count.ToString());

            Vector2 badgeSize = _collapseCountStyle.CalcSize(content);
            lpos.xMin = lpos.xMax - badgeSize.x;
            lpos.yMin += ((lpos.yMax - lpos.yMin) - badgeSize.y) * 0.5f;
            lpos.x -= 5f;
            EditorGUI.LabelField(lpos, content, _collapseCountStyle);

            return position;
        }

        private Rect RenderLogLevel(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;

            var content = new GUIContent(_logLevelLabel[(int)entry.level]);
            float min, max;
            _labelStyle.CalcMinMaxWidth(content, out min, out max);
            lpos.width = max;
            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Rect RenderFrameCount(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;

            var content = new GUIContent(string.Format("{0:D5}", entry.frame));
            float min, max;
            _labelStyle.CalcMinMaxWidth(content, out min, out max);
            lpos.width = max;
            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Rect RenderColor(Rect position, LogEntry entry, int index)
        {
            Rect lpos = position;
            lpos.width = ColorWidth;

            if (EnableLevelColors != null && EnableLevelColors[(int)entry.level])
            {
                Texture2D backgroundTexture = null;
                if (index % 2 == 0)
                {
                    backgroundTexture = TextureFromLogLevel(entry.level);
                }
                else
                {
                    backgroundTexture = DarkTextureFromLogLevel(entry.level);
                }
                if (Event.current.type == EventType.Repaint)
                {
                    GUI.DrawTexture(lpos, backgroundTexture);
                }
            }

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Rect RenderTimestampLabel(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;


            var content = new GUIContent(string.Format("{0:D3}:{1:D2}:{2:D3}", entry.duration.Minutes, entry.duration.Seconds, entry.duration.Milliseconds));
            float min, max;
            _labelStyle.CalcMinMaxWidth(content, out min, out max);
            lpos.width = min;
            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Rect RenderTextLabel(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.width = lpos.width - lpos.x;
            string content = entry.content;
            int crPos = content.IndexOf("\r\n");
            if (crPos < 0)
            {
                crPos = content.IndexOf('\n');
            }
            if (crPos >= 0)
            {
                content = content.Substring(0, crPos);
            }

            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Texture2D TextureFromLogLevel(LogLevel level)
        {
            if (_logLevelTextures == null)
            {
                _logLevelTextures = new Texture2D[CustomGUIStyle.LogLevelColors.Length];
                for (int i = 0; i < CustomGUIStyle.LogLevelColors.Length; ++i)
                {
                    _logLevelTextures[i] = new Texture2D(ColorWidth, 1);
                    SetTexturePixels(_logLevelTextures[i], CustomGUIStyle.LogLevelColors[i]);
                    _logLevelTextures[i].Apply();
                    _logLevelTextures[i].hideFlags = HideFlags.HideAndDontSave;
                }
            }
            return _logLevelTextures[(int)level];
        }

        private Texture2D DarkTextureFromLogLevel(LogLevel level)
        {
            if (_logLevelDarkTextures == null)
            {
                _logLevelDarkTextures = new Texture2D[CustomGUIStyle.LogLevelColors.Length];
                for (int i = 0; i < CustomGUIStyle.LogLevelColors.Length; ++i)
                {
                    _logLevelDarkTextures[i] = new Texture2D(ColorWidth, 1);
                    Color32 c = CustomGUIStyle.LogLevelColors[i];
                    c.r = (byte)Mathf.Clamp(c.r - 10, 0, 255);
                    c.g = (byte)Mathf.Clamp(c.g - 10, 0, 255); ;
                    c.b = (byte)Mathf.Clamp(c.b - 10, 0, 255); ;

                    SetTexturePixels(_logLevelDarkTextures[i], CustomGUIStyle.LogLevelColors[i]);
                    _logLevelDarkTextures[i].Apply();
                    _logLevelDarkTextures[i].hideFlags = HideFlags.HideAndDontSave;
                }
            }
            return _logLevelDarkTextures[(int)level];
        }

        private static void SetTexturePixels(Texture2D texture, Color32 color)
        {
            var start = color;
            var end = color;
            end.a = 0x0;
            for (int k = 0; k < ColorWidth; ++k)
            {
                texture.SetPixel(k, 0, Color32.Lerp(start, end, k / (float)ColorWidth));
            }
        }
    }
}
#endif