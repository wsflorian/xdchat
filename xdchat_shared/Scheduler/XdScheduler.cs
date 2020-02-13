using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XdChatShared.Scheduler {
    public class XdScheduler {
        private readonly SyncTaskScheduler mainThreadScheduler = new SyncTaskScheduler("MainThread");
        private readonly ConcurrentDictionary<int, Thread> asyncThreads = new ConcurrentDictionary<int, Thread>();

        public int QueuedSyncTasks => mainThreadScheduler.ScheduledTaskCount;
        public int RunningAsyncTasks => asyncThreads.Count;

        public void RunSync(Action action) {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.mainThreadScheduler);
        }

        public void RunAsync(string name, Action action) {
            Thread thread = new Thread(() => {
                action.Invoke();
                asyncThreads.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
            });
            
            asyncThreads[thread.ManagedThreadId] = thread;
            thread.Name = $"AsyncThread-{name}-{thread.ManagedThreadId}-";
            
            thread.Start();
        }
        
        public void CheckIsMainThread() {
            if (mainThreadScheduler.IsMainThread) return;
            throw new InvalidOperationException("Not running on main thread");
        }
    }
}