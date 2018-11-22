using System;
using System.Runtime.Serialization;

namespace Zob.Internal
{
    [Serializable]
    internal class ZobFatalException : Exception
    {
        public ZobFatalException()
        {
        }

        public ZobFatalException(string message) : base(message)
        {
        }

        public ZobFatalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ZobFatalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}