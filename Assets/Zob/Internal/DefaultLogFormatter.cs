using System.Text;

namespace Zob.Internal
{
    public sealed class DefaultLogFormatter : ILogFormatter
    {
        private StringBuilder sb = new StringBuilder();

        string ILogFormatter.Id { get { return "default"; } }

        string ILogFormatter.Format(LogEntry entry)
        {
            sb.Length = 0;
            if (entry.args != null)
            {
                sb.AppendFormat(entry.format, entry.args);
            }
            else
            {
                sb.AppendFormat(entry.format);
            }
            return sb.ToString();
        }
    }
}