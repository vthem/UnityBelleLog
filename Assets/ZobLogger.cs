using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Text;

namespace Zob
{

    public enum LogLevel
    {
        Trace = 1,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public struct LogEntry
    {
        public LogLevel level;
        public string format;
        public string domain;
        public object[] args;
        public System.DateTime timestamp;
    }

    public interface ILogWriter
    {
        void Open();
        void Write(string logEntryContent);
        void Close();

        string Id { get; }
    }

    public interface ILogFormatter
    {
        string Format(LogEntry entry);

        string Id { get; }
    }

    public sealed class DefaultLogFormatter : ILogFormatter
    {
        private StringBuilder sb = new StringBuilder();

        string ILogFormatter.Id { get { return "default"; } }

        string ILogFormatter.Format(LogEntry entry)
        {
            sb.Length = 0;
            sb.AppendFormat(entry.format, entry.args);
            return sb.ToString();
        }
    }

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
            }
        }

        private string CreateDirectory()
        {
            string absoluteDirectory;
#if UNITY_EDITOR
            absoluteDirectory = Path.Combine(Application.dataPath, LogSystem.TemporaryFolder);
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
            string absolutePath = Path.Combine(absoluteDirectory, "log.txt");
            fileStream = File.Open(absolutePath, FileMode.Append);
        }
    }

    public sealed class LogHandler
    {
        private Queue<LogEntry> entryQueue = new Queue<LogEntry>();
        private AutoResetEvent entryQueueSignal = new System.Threading.AutoResetEvent(false);
        private Thread handlerThread;
        private List<ILogWriter> writerList = new List<ILogWriter>();
        private List<ILogFormatter> formatterList = new List<ILogFormatter>();

        public void PushLogEntry(LogEntry entry)
        {
            lock (entryQueue)
            {
                entryQueue.Enqueue(entry);
            }
        }

        public void Start()
        {
            handlerThread = new Thread(ThreadLoop);
        }

        public void Stop()
        {
            if (handlerThread != null)
            {
                handlerThread.Abort();
            }
        }

        public void AddWriter(ILogWriter writer)
        {
            lock (writerList)
            {
                writerList.Add(writer);
            }
        }

        public void AddFormatter(ILogFormatter formatter)
        {
            lock (writerList)
            {
                formatterList.Add(formatter);
            }
        }

        private void ThreadLoop()
        {
            while (true)
            {
                entryQueueSignal.WaitOne();
                LogEntry entry;
                lock (entryQueue)
                {
                    if (entryQueue.Count == 0)
                    {
                        continue;
                    }
                    entry = entryQueue.Dequeue();
                }

                lock (writerList)
                {
                    for (int k = 0; k < formatterList.Count; ++k)
                    {
                        string content = formatterList[k].Format(entry);
                        for (int i = 0; i < writerList.Count; ++i)
                        {
                            writerList[i].Write(content);
                        }
                    }
                }
            }
        }
    }

    public abstract class LogSingleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

    public struct LogConfigEntry
    {
        public LogLevel minLogLevel;
        public bool isEnable;
    }

    [System.Serializable]
    public class LogConfigDomainFilter
    {
        public string domainMatch;
        public List<string> restrictWriter;
    }

    [System.Serializable]
    public class LogConfigModel
    {
        public bool useThread;
        public List<LogConfigDomainFilter> filters;
    }

    public class LogConfig : LogSingleton<LogConfig>
    {
        private LogConfigModel _model;

        public LogConfig()
        {
        }

        public LogConfigEntry GetEntry(string domain)
        {
            LogConfigEntry entry;
            entry.isEnable = false;
            entry.minLogLevel = LogLevel.Fatal;
            return entry;
        }

        public static LogConfigModel DefaultModel
        {
            get
            {
                var model = new LogConfigModel();
                model.filters = new List<LogConfigDomainFilter>();

                var filter = new LogConfigDomainFilter();
                filter.domainMatch = "*";
                model.filters.Add(filter);
                return model;
            }
        }
    }

    public sealed class LogSystem : LogSingleton<LogSystem>
    {
        public const string AssetRootFolder = "ZobLogger";
        public static string ResourcesFolder { get { return System.IO.Path.Combine(AssetRootFolder, "Resources"); } }
        public static string TemporaryFolder { get { return System.IO.Path.Combine(AssetRootFolder, "Temporary"); } }

        private bool _initialized = false;
        public bool IsInitialized { get { return _initialized; } }

        private LogHandler logHandler = new LogHandler();

        public void Log(LogEntry entry)
        {
            if (!_initialized)
            {
                return;
            }
            logHandler.PushLogEntry(entry);
        }

        public void Initialize(LogConfigModel config)
        {
            _initialized = true;
            ILogWriter writer = new FileLogWriter();
            writer.Open();
            logHandler.AddWriter(writer);

            ILogFormatter formatter = new DefaultLogFormatter();
            logHandler.AddFormatter(formatter);

            logHandler.Start();
        }
    }

    public sealed class Logger
    {
        private string _domain;
        private LogSystem _logSystem;

        public Logger(string domain)
        {
            _domain = domain;
            _logSystem = LogSystem.Instance;
        }

        public void Trace()
        {
            if (!_logSystem.IsInitialized)
            {
                return;
            }
            LogEntry entry;
            entry.args = null;
            entry.format = "Trace function";
            entry.level = LogLevel.Trace;
            entry.domain = _domain;
            entry.timestamp = System.DateTime.Now;
            _logSystem.Log(entry);
        }

        private void InitializeOnce()
        {

        }
    }
}