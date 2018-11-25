using System.IO;
using System.Text;
using UnityEngine;

namespace BelleLog.Internal
{
    public sealed class FileLogWriter : ILogWriter
    {
        private FileStream _fileStream;
        private string _logFilename;

        public FileLogWriter(string logFilename)
        {
            _logFilename = logFilename;
        }

        string ILogWriter.Id
        {
            get
            {
                return "default";
            }
        }

        void ILogWriter.Close()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
            }
        }

        void ILogWriter.Open()
        {
            string absoluteDirectory = CreateDirectory();
            OpenLogFile(absoluteDirectory);
        }

        void ILogWriter.Write(string logEntryContent)
        {

            if (_fileStream != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(logEntryContent);
                _fileStream.Write(data, 0, data.Length);
                _fileStream.Flush();
            }
        }

        private string CreateDirectory()
        {
            //            string absoluteDirectory;
            //#if UNITY_EDITOR
            //            absoluteDirectory = Path.GetDirectoryName(Application.dataPath);
            //            absoluteDirectory = Path.Combine(absoluteDirectory, "Logs");
            //#else
            //            absoluteDirectory = Path.Combine(Application.persistentDataPath, LogSystem.TemporaryFolder);
            //#endif
            //            if (!Directory.Exists(absoluteDirectory))
            //            {
            //                Directory.CreateDirectory(absoluteDirectory);
            //            }
            //            return absoluteDirectory;
            return string.Empty;
        }

        private void OpenLogFile(string absoluteDirectory)
        {
            string logFilePath = Path.Combine(absoluteDirectory, _logFilename);
            _fileStream = File.Open(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
        }
    }
}