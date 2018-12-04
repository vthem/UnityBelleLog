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

        private string[] _logLevelLabel = new string[]
        {
            "TRC",
            "DBG",
            "NFO",
            "WRN",
            "ERR",
            "FTL"
        };

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
            if (Event.current.type == EventType.Repaint)
            {
                if (EnableLevelColors != null && EnableLevelColors[(int)entry.level])
                {
                    if (index % 2 == 0)
                    {
                        backgroundTexture = CustomGUIStyle.TextureFromLogLevel(entry.level);
                    }
                    else
                    {
                        backgroundTexture = CustomGUIStyle.DarkTextureFromLogLevel(entry.level);
                    }
                }
                else
                {
                    if (index % 2 == 0)
                    {
                        backgroundTexture = _oddBackgroundTexture;
                    }
                    else
                    {
                        backgroundTexture = _evenBackgroundTexture;
                    }
                }
            }
            GUI.DrawTexture(position, backgroundTexture);

            var fontSize = _labelStyle.fontSize;
            var fontStyle = _labelStyle.fontStyle;
            bool entrySelected = index == selectedIndex;
            if (entrySelected)
            {
                _labelStyle.fontSize += 1;
                _labelStyle.fontStyle = FontStyle.Bold;
            }
            if (_collapseFilter.Enable)
            {
                position = RenderCollapseCount(position, index);
            }
            position = RenderTimestampLabel(position, entry);
            position = RenderFrameCount(position, entry);
            position = RenderLogLevel(position, entry);
            position = RenderTextLabel(position, entry);
            _labelStyle.fontSize = fontSize;
            _labelStyle.fontStyle = fontStyle;
        }

        private bool IndexValid(int index)
        {
            return index >= 0 && index < _container.Count;
        }

        private Rect RenderCollapseCount(Rect position, int index)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;

            var count = _collapseFilter.CollapseCount(index);
            GUIContent content = null;
            if (count == 0)
            {
                content = new GUIContent(string.Format(" - "));
            }
            else if (count < 999)
            {
                content = new GUIContent(string.Format("{0:D3}", _collapseFilter.CollapseCount(index)));
            }
            else
            {
                content = new GUIContent(string.Format("+++"));
            }
            float min, max;
            _labelStyle.CalcMinMaxWidth(content, out min, out max);
            lpos.width = max;
            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
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
    }
}