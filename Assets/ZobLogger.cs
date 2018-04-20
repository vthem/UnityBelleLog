using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Zob {

    public enum LogLevel {
        Trace = 1,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public struct LogEntry {
        public LogLevel level;
        public string format;
        public string domain;
        public object[] args;
        public System.DateTime timestamp;
    }

    public interface ILogWriter {
        void Open();
        void Write(LogEntry entry);
        void Close();
    }

    public sealed class LogHandler {
        private Queue<LogEntry> entryQueue = new Queue<LogEntry>();
        private AutoResetEvent entryQueueSignal = new System.Threading.AutoResetEvent(false);
        private Thread handlerThread;
        private List<ILogWriter> writerList = new List<ILogWriter>();

        public void PushLogEntry(LogEntry entry) {
            lock (entryQueue) {
                entryQueue.Enqueue(entry);
            }
        }

        public void Start() {
            handlerThread = new Thread(ThreadLoop);
        }

        public void Stop() {
            if (handlerThread != null) {
                handlerThread.Abort();
            }
        }

        private void ThreadLoop() {
            while (true) {
                entryQueueSignal.WaitOne();
                LogEntry entry;
                lock (entryQueue) {
                    if (entryQueue.Count == 0) {
                        continue;
                    }
                    entry = entryQueue.Dequeue();
                }

                lock (writerList) {
                    for (int i = 0; i < writerList.Count; ++i) {
                        writerList[i].Write(entry);
                    }
                }
            }
        }
    }

    public abstract class LogSingleton<T> where T : new() {
        private static T _instance;

        public static T Instance
        {
            get {
                if (_instance == null) {
                    _instance = new T();
                }
                return _instance;
            }
        }
    }

    public struct LogConfigEntry {
        public LogLevel minLogLevel;
        public bool isEnable;
    }

    [System.Serializable]
    public class LogConfigDomainFilter {
        public string domainMatch;
        public List<string> restrictWriter;
    }

    [System.Serializable]
    public class LogConfigModel {
        public bool useThread;
        public List<LogConfigDomainFilter> filters;
    }

    public class LogConfig : LogSingleton<LogConfig> {
        private LogConfigModel _model;

        public LogConfig() {
            Debug.Log("LogConfig::");
            //var config = Resources.Load<ScriptableObject>("LogConfig.asset");
            Debug.Log("==> " + JsonUtility.ToJson(LogConfig.DefaultModel));
            string logFilePath;
#if UNITY_EDITOR
            logFilePath = System.IO.Path.Combine(Application.dataPath, "LogConfig.json");
#else
            logFilePath = System.IO.Path.Combine(Application.persistentDataPath, "LogConfig.json");
#endif
            if (!System.IO.File.Exists(logFilePath)) {
                System.IO.File.WriteAllText(logFilePath, JsonUtility.ToJson(DefaultModel, true));
                _model = DefaultModel;
            }
            else {
                _model = JsonUtility.FromJson<LogConfigModel>(System.IO.File.ReadAllText(logFilePath));
            }
        }

        public LogConfigEntry GetEntry(string domain) {
            LogConfigEntry entry;
            entry.isEnable = false;
            entry.minLogLevel = LogLevel.Fatal;
            return entry;
        }

        public static LogConfigModel DefaultModel
        {
            get {
                var model = new LogConfigModel();
                model.configType = "ahaha";
                model.filters = new List<LogConfigDomainFilter>();

                var filter = new LogConfigDomainFilter();
                filter.domainMatch = "/*/";
                model.filters.Add(filter);
                return model;
            }
        }
    }

    public sealed class LogSystem : LogSingleton<LogSystem> {
        private bool _initialized = false;
        public bool IsInitialized { get { return _initialized; } }

        public void Log(LogEntry entry) {
            if (!_initialized) {
                return;
            }
        }

        public void Initialize(LogConfigModel config) {

            _initialized = true;
        }
    }

    public sealed class Logger {
        private string _domain;
        private LogSystem _logSystem;

        public Logger(string domain) {
            _domain = domain;
            _logSystem = LogSystem.Instance;
        }

        public void Trace() {
            if (!_logSystem.IsInitialized) {
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

        private void InitializeOnce() {

        }
    }
}