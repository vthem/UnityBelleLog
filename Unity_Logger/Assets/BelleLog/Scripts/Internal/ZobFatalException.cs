using System;
using System.Runtime.Serialization;

namespace BelleLog.Internal
{
    [Serializable]
    internal class BelleLogFatalException : Exception
    {
        public BelleLogFatalException()
        {
        }

        public BelleLogFatalException(string message) : base(message)
        {
        }

        public BelleLogFatalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}