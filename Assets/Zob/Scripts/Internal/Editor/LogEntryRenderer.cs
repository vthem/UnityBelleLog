using System;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class LogEntryRenderer : ITableLineRenderer
    {
        private ILogEntryContainer _container;
        private GUIStyles _styles;
        private DateTime _startTimestamp;
        private const string _defaultTimestampLabel = "0m00s000";
        private GUIStyle _labelStyle;
        private Color32[] _levelColors = new Color32[]
        {
            new Color32(181, 230, 29, 0xff),
            new Color32(153, 217, 234, 0xff),
            new Color32(255, 255, 255, 0xff),
            new Color32(255, 201, 14, 0xff),
            new Color32(237, 28, 36, 0xff),
            new Color32(206, 0, 190, 0xff),
        };
        private int lastRenderIndex = -1;

        public bool[] EnableLevelColors { get; set; }

        public LogEntryRenderer(ILogEntryContainer container, GUIStyles styles)
        {
            _container = container;
            _styles = styles;
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.alignment = TextAnchor.MiddleLeft;
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
                GUIStyle s = index % 2 == 0 ? _styles.OddBackground : _styles.EvenBackground;
                s.Draw(position, false, false, entrySelected, false);
            }

            var entry = _container[index];

            var normalColor = _labelStyle.normal.textColor;
            if (EnableLevelColors != null && EnableLevelColors[(int)entry.level])
            {
                _labelStyle.normal.textColor = _levelColors[(int)entry.level];
            }
            position = RenderFrameCount(position, entry);
            position = RenderTimestampLabel(position, entry);
            position = RenderTextLabel(position, entry);
            _labelStyle.normal.textColor = normalColor;

            lastRenderIndex = index;
        }

        private bool IndexValid(int index)
        {
            return index >= 0 || index < _container.Count;
        }

        private Rect RenderFrameCount(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;

            var content = new GUIContent(string.Format("{0:D5}", entry.frameCount));
            float min, max;
            GUI.skin.label.CalcMinMaxWidth(content, out min, out max);
            lpos.width = max;
            EditorGUI.LabelField(lpos, content, _labelStyle);

            position.x = lpos.x + lpos.width;
            return position;
        }

        private Rect RenderTimestampLabel(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 3;


            var content = new GUIContent(string.Format("{0:D3}m{1:D2}s{2:D3}", entry.duration.Minutes, entry.duration.Seconds, entry.duration.Milliseconds));
            float min, max;
            GUI.skin.label.CalcMinMaxWidth(content, out min, out max);
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