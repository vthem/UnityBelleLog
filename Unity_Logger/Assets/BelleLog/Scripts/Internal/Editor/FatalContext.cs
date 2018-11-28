using System;
using UnityEngine;

namespace BelleLog.Internal.Editor
{
    public class FatalContext
    {
        public static FatalContext Instance { get { return LogSingleton<FatalContext>.Instance; } }

        public bool IsSet { get; private set; }

        public Event CurrentEvent
        {
            get; private set;
        }

        public Exception CurrentException
        {
            get; private set;
        }

        public void Reset()
        {
            IsSet = false;
            CurrentEvent = null;
            CurrentException = null;
        }

        internal void Add(Event currentEvent, Exception currentException)
        {
            CurrentEvent = currentEvent;
            CurrentException = currentException;
            IsSet = true;
        }
    }
}