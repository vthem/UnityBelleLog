
using System.Collections.Generic;
using System.Threading;

namespace Zob.Internal
{
    public sealed class LogHandler
    {
        private Queue<LogEntry> entryQueue = new Queue<LogEntry>();
        private AutoResetEvent entryQueueSignal = new System.Threading.AutoResetEvent(false);
        private Thread handlerThread;
        private List<ILogWriter> writerList = new List<ILogWriter>();
        private List<ILogFormatter> formatterList = new List<ILogFormatter>();

        public void Enqueue(LogEntry entry)
        {
            lock (entryQueue)
            {
                entryQueue.Enqueue(entry);
                entryQueueSignal.Set();
            }
        }

        public void Start()
        {
            handlerThread = new Thread(ThreadLoop);
            handlerThread.Start();
        }

        public void Stop()
        {
            if (handlerThread != null)
            {
                handlerThread.Abort();
                handlerThread = null;
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
}