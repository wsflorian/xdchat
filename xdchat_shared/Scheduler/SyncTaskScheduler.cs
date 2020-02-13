using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XdChatShared.Scheduler {
    public class SyncTaskScheduler : TaskScheduler, IDisposable {
        private readonly ConcurrentQueue<Task> tasks = new ConcurrentQueue<Task>();

        private readonly Thread workingThread;

        private readonly AutoResetEvent signalThread = new AutoResetEvent(false);

        private bool stopThread;

        private bool disposed;

        public SyncTaskScheduler(string name) {
            workingThread = new Thread(RunThread) {Name = name};
            workingThread.Start();
        }
        
        protected sealed override void QueueTask(Task task) {
            if (stopThread) {
                throw new InvalidOperationException("Scheduler can't handle new tasks, because it has been stopped.");
            }

            tasks.Enqueue(task);
            signalThread.Set();
        }

        private void RunThread() {
            while (true) {
                if (stopThread && tasks.IsEmpty)
                    return;

                signalThread.WaitOne();

                while (tasks.TryDequeue(out Task item)) {
                    this.TryExecuteTask(item);
                }
            }
        }

        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) {
            return this.IsMainThread && base.TryExecuteTask(task);
        }
        
        public sealed override int MaximumConcurrencyLevel => 1;

        protected sealed override IEnumerable<Task> GetScheduledTasks() {
            bool lockTaken = false;
            try {
                Monitor.TryEnter(tasks, ref lockTaken);
                if (lockTaken)
                    return tasks.ToArray();
                else
                    throw new NotSupportedException("Tasks queue can't be locked");
            }
            finally {
                if (lockTaken) Monitor.Exit(tasks);
            }
        }

        public int ScheduledTaskCount => tasks.Count;

        public bool IsMainThread => Thread.CurrentThread.ManagedThreadId == workingThread.ManagedThreadId;

        #region IDisposable implementation

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposed)
                return;

            if (disposing) {
                stopThread = true;
                signalThread.Set();
            }

            disposed = true;
        }

        #endregion
    }
}