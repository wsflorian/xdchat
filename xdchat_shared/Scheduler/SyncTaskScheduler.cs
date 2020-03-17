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
        
        private readonly AutoResetEvent queueSignal = new AutoResetEvent(false);

        private bool stopThread, disposed;
        
        private volatile WatchdogInfo watchdogInfo;

        public SyncTaskScheduler(string name) {
            workingThread = new Thread(RunWorkingThread) {Name = name};
            workingThread.Start();

            Thread watchdogThread = new Thread(RunWatchdogThread) {Name = $"{name}-watchdog"};
            watchdogThread.Start();
        }
        
        protected sealed override void QueueTask(Task task) {
            if (stopThread) {
                throw new InvalidOperationException("Scheduler can't handle new tasks, because it has been stopped.");
            }

            tasks.Enqueue(task);
            queueSignal.Set();
        }

        private void RunWorkingThread() {
            while (true) {
                if (stopThread && tasks.IsEmpty)
                    return;
                
                queueSignal.WaitOne();

                while (tasks.TryDequeue(out Task result)) {
                    watchdogInfo = new WatchdogInfo();
                    
                    this.TryExecuteTask(result);
                    
                    watchdogInfo = null;
                }
            }
        }

        private void RunWatchdogThread() {
            while (true) {
                if (stopThread && tasks.IsEmpty)
                    return;
                
                while (this.watchdogInfo != null) {
                    this.watchdogInfo?.CheckStuck();
                    Thread.Sleep(5);
                }
                
                Thread.Sleep(5);
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
                queueSignal.Set();
            }

            disposed = true;
        }

        #endregion
    }

    class WatchdogInfo {
        private readonly long lastExecutionStart;
        private bool notified;
        
        public WatchdogInfo() {
            this.lastExecutionStart = XdScheduler.CurrentTimeMillis();
        }

        public void CheckStuck() {
            if (notified || !IsStuck()) return;
            
            notified = true;
            Console.Error.WriteLine("WorkingThread task takes >1s to execute");
        }

        private bool IsStuck() {
            return lastExecutionStart + 1000 < XdScheduler.CurrentTimeMillis();
        }
    }
}