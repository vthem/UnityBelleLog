using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Zob.Internal
{
    public interface ILogHandler
    {
        void Enqueue(LogEntry entry);
    }
}