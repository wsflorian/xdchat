using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XdChatShared.Scheduler {
    public class SyncTaskScheduler : TaskScheduler, IDisposable {
        private readonly ConcurrentQueue<Task> tasks = new ConcurrentQueue<Task>();
        private readonly Thread _workingThread;
        
        private readonly AutoResetEvent _queueSignal = new AutoResetEvent(false);

        private bool _stopThread, _disposed;
        
        private volatile WatchdogInfo _watchdogInfo;

        public SyncTaskScheduler(string name) {
            _workingThread = new Thread(RunWorkingThread) {Name = name};
            _workingThread.Start();

            Thread watchdogThread = new Thread(RunWatchdogThread) {Name = $"{name}-watchdog"};
            watchdogThread.Start();
        }
        
        protected sealed override void QueueTask(Task task) {
            if (_stopThread) {
                throw new InvalidOperationException("Scheduler can't handle new tasks, because it has been stopped.");
            }

            tasks.Enqueue(task);
            _queueSignal.Set();
        }

        private void RunWorkingThread() {
            while (true) {
                if (_stopThread && tasks.IsEmpty)
                    return;
                
                _queueSignal.WaitOne();

                while (tasks.TryDequeue(out Task result)) {
                    _watchdogInfo = new WatchdogInfo();
                    
                    this.TryExecuteTask(result);
                    
                    _watchdogInfo = null;
                }
            }
        }

        private void RunWatchdogThread() {
            while (true) {
                if (_stopThread && tasks.IsEmpty)
                    return;
                
                while (this._watchdogInfo != null) {
                    this._watchdogInfo?.CheckStuck();
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

        public bool IsMainThread => Thread.CurrentThread.ManagedThreadId == _workingThread.ManagedThreadId;

        #region IDisposable implementation

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;

            if (disposing) {
                _stopThread = true;
                _queueSignal.Set();
            }

            _disposed = true;
        }

        #endregion
    }

    class WatchdogInfo {
        private readonly DateTime lastExecutionStart;
        private bool notified;
        
        public WatchdogInfo() {
            this.lastExecutionStart = DateTime.Now;
        }

        public void CheckStuck() {
            if (notified || !IsStuck()) return;
            
            notified = true;
            Console.Error.WriteLine("WorkingThread task takes >1s to execute");
        }

        private bool IsStuck() {
            return lastExecutionStart.AddSeconds(1) < DateTime.Now;
        }
    }
}