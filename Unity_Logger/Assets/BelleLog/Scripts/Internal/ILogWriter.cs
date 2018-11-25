namespace BelleLog.Internal
{
    public interface ILogWriter
    {
        void Open();
        void Write(string logEntryContent);
        void Close();

        string Id { get; }
    }
}