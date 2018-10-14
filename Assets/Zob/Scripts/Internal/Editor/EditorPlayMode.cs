using UnityEditor;
using System;

namespace Zob.Internal
{
    public enum PlayModeState
    {
        Stopped,
        Playing,
        Paused
    }

    [InitializeOnLoad]
    public class EditorPlayMode
    {
        private static PlayModeState _currentState = PlayModeState.Stopped;

        static EditorPlayMode()
        {
            EditorApplication.playmodeStateChanged = EditorApplicationPlayModeChangedHandler;
            if (EditorApplication.isPaused)
            {
                _currentState = PlayModeState.Paused;
            }
        }

        public static event Action<PlayModeState, PlayModeState> PlayModeChanged;
        public static DateTime StartTime { get; private set; }

        private static void OnPlayModeChanged(PlayModeState currentState, PlayModeState changedState)
        {
            if (PlayModeChanged != null)
            {
                PlayModeChanged(currentState, changedState);
            }
        }

        private static void EditorApplicationPlayModeChangedHandler()
        {
            var changedState = PlayModeState.Stopped;
            switch (_currentState)
            {
                case PlayModeState.Stopped:
                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    changedState = PlayModeState.Playing;
                    StartTime = DateTime.Now;
                }
                else if (EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Paused;
                }
                break;
                case PlayModeState.Playing:
                if (EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Paused;
                }
                else if (EditorApplication.isPlaying)
                {
                    changedState = PlayModeState.Playing;
                    StartTime = DateTime.Now;
                }
                else
                {
                    changedState = PlayModeState.Stopped;
                }
                break;
                case PlayModeState.Paused:
                if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Playing;
                    StartTime = DateTime.Now;
                }
                else if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPaused)
                {
                    changedState = PlayModeState.Paused;
                }
                break;
                default:
                throw new ArgumentOutOfRangeException();
            }

            // Fire PlayModeChanged event.
            if (_currentState != changedState)
            {
                OnPlayModeChanged(_currentState, changedState);
            }

            // Set current state.
            _currentState = changedState;
        }

    }
}