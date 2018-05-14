using System;
using UnityEditor;
using UnityEngine;

namespace Zob.Internal.Editor
{
    public interface IPosition
    {
        Rect Position { get; }
    }

    public abstract class ChildGUI : IPosition
    {
        protected Rect _position;
        protected IPosition _parent;
        public bool Repaint { get; protected set; }

        public ChildGUI(IPosition parent)
        {
            _parent = parent;
            _position = _parent.Position;
        }

        public Rect Position { get { return _position; } }

        public abstract void OnGUI();
    }

    public class SeparationLine : ChildGUI
    {
        protected Texture2D _greyTex;

        public SeparationLine(IPosition parent) : base(parent)
        {
            _greyTex = new Texture2D(1, 1);
            _greyTex.SetPixel(0, 0, Color.cyan);
            _greyTex.Apply();
            _position.y = 50;
            _position.height = 1;
        }

        public override void OnGUI()
        {
            if (Event.current.type == EventType.MouseDrag)
            {
                _position.y = Event.current.mousePosition.y;
                Repaint = true;
            }
            GUI.DrawTexture(_position, _greyTex);
            var overRect = _position;
            overRect.height = overRect.height + 3;
            EditorGUIUtility.AddCursorRect(overRect, MouseCursor.ResizeVertical);
        }
    }

    public class Console : EditorWindow, IPosition
    {
        private Vector2 scrollPos;
        private int count = 0;
        private int texSize = 10;
        private Rect _rect;
        private float _scrollValue;
        private float _rowHeight = 30f;
        private bool _initialized = false;
        private ConsoleConfig _config;
        private SeparationLine _seperationLine;

        Rect IPosition.Position
        {
            get
            {
                return position;
            }
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/ZobConsole")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            var console = (Console)EditorWindow.GetWindow(typeof(Console));
            console.InitializeOnce();
            console.Show();
            Debug.Log("open zop console window");
        }

        private void InitializeOnce()
        {
            if (_initialized)
            {
                return;
            }
            wantsMouseMove = true;
            titleContent = new GUIContent("ZobConsole");
            _config = ConsoleConfig.Load();
            _seperationLine = new SeparationLine(this);
            _initialized = true;
        }

        void OnGUI()
        {
            InitializeOnce();
            //EditorGUILayout.BeginHorizontal();
            //scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            //GUILayout.BeginArea(new Rect(0, 0, texSize, texSize), Texture2D.whiteTexture);
            //GUILayout.EndArea();
            //for (int i = 0; i < count; ++i)
            //{
            //    GUILayout.Label(i.ToString());
            //}
            //EditorGUILayout.EndScrollView();
            //EditorGUILayout.EndHorizontal();
            //if (GUILayout.Button("+10"))
            //    count += 10;
            //if (GUILayout.Button("+10 tex"))
            //    texSize += 10;
            //EditorGUILayout.LabelField(scrollPos.ToString());
            //_rect = EditorGUILayout.RectField(_rect);
            //GUI.Label(_rect, "test");
            //EditorGUILayout.RectField("position", _window.position);
            //EditorGUILayout.Vector2Field("delta", Event.current.delta);
            //for (int i = 0; i < 10; ++i)
            //{
            //    GUI.DrawTexture(new Rect(0, i * _rowHeight, _window.position.width - GUI.skin.verticalScrollbar.fixedWidth, _rowHeight), Texture2D.whiteTexture);
            //}

            if (Event.current.type != EventType.Layout)
                Debug.Log("event=" + Event.current.type);

            _seperationLine.OnGUI();
            if (_seperationLine.Repaint)
            {
                Repaint();
            }
            EditorGUILayout.Vector2Field("mouse", Event.current.mousePosition);
            GUI.DrawTexture(new Rect(0, 0, 100, 100), Texture2D.blackTexture);
            //GUI.DrawTexture(new Rect(50, 50, 150, 150), Texture2D.blackTexture);


            //if (Event.current.type == EventType.ScrollWheel)
            //{
            //    _scrollValue += Event.current.delta.y;
            //    Mathf.Clamp(_scrollValue, 0, 10);
            //    Repaint();
            //}

        }
    }
}