using System.IO;
using System.Text;
using UnityEngine;
using Zob.Internal;

namespace Zob.Writer
{
    public sealed class FileLogWriter : ILogWriter
    {
        private FileStream fileStream;

        string ILogWriter.Id
        {
            get
            {
                return "default";
            }
        }

        void ILogWriter.Close()
        {
            if (fileStream != null)
            {
                fileStream.Close();
            }
        }

        void ILogWriter.Open()
        {
            string absoluteDirectory = CreateDirectory();
            OpenLogFile(absoluteDirectory);
        }

        void ILogWriter.Write(string logEntryContent)
        {

            if (fileStream != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(logEntryContent);
                fileStream.Write(data, 0, data.Length);
                fileStream.Flush();
            }
        }

        private string CreateDirectory()
        {
            string absoluteDirectory;
#if UNITY_EDITOR
            absoluteDirectory = Path.GetDirectoryName(Application.dataPath);
            absoluteDirectory = Path.Combine(absoluteDirectory, "Logs");
#else
            absoluteDirectory = Path.Combine(Application.persistentDataPath, LogSystem.TemporaryFolder);
#endif
            if (!Directory.Exists(absoluteDirectory))
            {
                Directory.CreateDirectory(absoluteDirectory);
            }
            return absoluteDirectory;
        }

        private void OpenLogFile(string absoluteDirectory)
        {
            string logFilePath = Path.Combine(absoluteDirectory, "default.log");
            fileStream = File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        }
    }
}