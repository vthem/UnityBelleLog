using System;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    internal class LogEntryRenderer : ITableLineRenderer
    {
        private ILogEntryContainer _container;
        private GUIStyles _styles;
        private Texture2D[] _levelTexture;
        private DateTime _startTimestamp;
        private const string _defaultTimestampLabel = "0m00s000";
        private GUIStyle _labelStyle;

        public LogEntryRenderer(ILogEntryContainer container, GUIStyles styles)
        {
            _container = container;
            _styles = styles;
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.alignment = TextAnchor.MiddleLeft;

            _levelTexture = new Texture2D[]
            {
                new Texture2D(1, 1), // TRC
                new Texture2D(1, 1), // DBG
                new Texture2D(1, 1), // NFO
                new Texture2D(1, 1), // WRN
                new Texture2D(1, 1), // ERR
                new Texture2D(1, 1)  // FTL
            };
            _levelTexture[(int)LogLevel.Trace].SetPixel(0, 0, new Color32(195, 195, 195, 0xff));
            _levelTexture[(int)LogLevel.Debug].SetPixel(0, 0, new Color32(240, 240, 240, 0xff));
            _levelTexture[(int)LogLevel.Info].SetPixel(0, 0, new Color32(160, 221, 160, 0xff));
            _levelTexture[(int)LogLevel.Warning].SetPixel(0, 0, new Color32(255, 128, 0, 0xff));
            _levelTexture[(int)LogLevel.Error].SetPixel(0, 0, new Color32(255, 89, 89, 0xff));
            _levelTexture[(int)LogLevel.Fatal].SetPixel(0, 0, new Color32(255, 0, 0, 0xff));

            for (int i = 0; i < _levelTexture.Length; ++i)
            {
                _levelTexture[i].Apply();
                _levelTexture[i].hideFlags = HideFlags.HideAndDontSave;
            }
        }

        public void OnGUI(Rect position, int index, int selectedIndex)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIStyle s = index % 2 == 0 ? _styles.OddBackground : _styles.EvenBackground;
                s.Draw(position, false, false, selectedIndex == index, false);
            }

            var entry = _container[index];

            position = RenderLevelColor(position, entry);
            position = RenderFrameCount(position, entry);
            position = RenderTimestampLabel(position, entry);
            position = RenderTextLabel(position, entry);
        }

        private Rect RenderLevelColor(Rect position, LogEntry entry)
        {
            Rect lpos = position;
            lpos.x = position.x + 4;
            lpos.width = 4;
            lpos.y = position.y + 6;
            lpos.height = 8;

            GUI.DrawTexture(lpos, _levelTexture[(int)entry.level]);

            position.x = lpos.x + lpos.width;
            return position;
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