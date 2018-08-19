
using System.Collections.Generic;
using System.Threading;

namespace Zob.Internal
{
    internal sealed class LogHandler : ILogHandler
    {
        private Queue<LogEntry> entryQueue = new Queue<LogEntry>();
        private AutoResetEvent entryQueueSignal = new System.Threading.AutoResetEvent(false);
        private Thread handlerThread;

        void ILogHandler.Enqueue(LogEntry entry)
        {
            lock (entryQueue)
            {
                entryQueue.Enqueue(entry);
                entryQueueSignal.Set();
            }
        }

        private void Start()
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

        private void ThreadLoop()
        {
            //while (true)
            //{
            //    entryQueueSignal.WaitOne();
            //    LogEntry entry;
            //    lock (entryQueue)
            //    {
            //        if (entryQueue.Count == 0)
            //        {
            //            continue;
            //        }
            //        entry = entryQueue.Dequeue();
            //    }

            //    lock (Writer)
            //    {
            //        for (int k = 0; k < Formatter.Count; ++k)
            //        {
            //            string content = Formatter[k].Format(entry);
            //            for (int i = 0; i < Writer.Count; ++i)
            //            {
            //                Writer[i].Write(content);
            //            }
            //        }
            //    }
            //}
        }
    }
}