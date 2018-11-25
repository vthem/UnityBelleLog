using UnityEditor;
using UnityEngine;
using Zob.Internal.Editor.Filter;

namespace Zob.Internal.Editor
{
    internal class LogEntryRenderer : ITableLineRenderer
    {
        private ILogEntryContainer _container;
        private const string _defaultTimestampLabel = "0m00s000";
        private GUIStyle _labelStyle;
        private readonly GUIStyle _oddBackgroundStyle;
        private readonly GUIStyle _evenBackgroundStyle;
        private Color32[] _levelColors = new Color32[]
        {
            new Color32(181, 230, 29, 0xff),
            new Color32(153, 217, 234, 0xff),
            new Color32(255, 255, 255, 0xff),
            new Color32(255, 201, 14, 0xff),
            new Color32(237, 28, 36, 0xff),
            new Color32(206, 0, 190, 0xff),
        };
        private CollapseLogFilter _collapseFilter;

        public bool[] EnableLevelColors { get; set; }

        public LogEntryRenderer(ILogEntryContainer container, CollapseLogFilter collapseFilter)
        {
            _container = container;
            _labelStyle = new GUIStyle(GUI.skin.label);
            _evenBackgroundStyle = new GUIStyle("CN EntryBackEven");
            _oddBackgroundStyle = new GUIStyle("CN EntryBackodd");
            _labelStyle.alignment = TextAnchor.MiddleLeft;
            CustomGUIStyle.SetConsoleFont(_labelStyle);
            _collapseFilter = collapseFilter;
        }

        public Color32 GetLevelColor(LogLevel level)
        {
            return _levelColors[(int)level];
        }

        public void OnGUI(Rect position, int index, int selectedIndex)
        {
            if (!IndexValid(index))
            {
                return;
            }

            bool entrySelected = index == selectedIndex;
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle s = index % 2 == 0 ? _oddBackgroundStyle : _evenBackgroundStyle;
                s.Draw(position, false, false, entrySelected, false);
            }

            var entry = _container[index];

            var normalColor = _labelStyle.normal.textColor;
            if (EnableLevelColors != null && EnableLevelColors[(int)entry.level])
            {
                _labelStyle.normal.textColor = _levelColors[(int)entry.level];
            }
            if (_collapseFilter.Enable)
            {
                position = RenderCollapseCount(position, index);
            }
            position = RenderFrameCount(position, entry);
            position = RenderTimestampLabel(position, entry);
            position = RenderTextLabel(position, entry);
            _labelStyle.normal.textColor = normalColor;
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
            position.x = position.x + 15;
            EditorGUI.LabelField(lpos, entry.content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }
    }
}