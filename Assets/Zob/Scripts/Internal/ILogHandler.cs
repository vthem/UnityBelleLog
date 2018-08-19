using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Zob.Internal
{
    internal interface ILogHandler
    {
        void Enqueue(LogEntry entry);
    }
}