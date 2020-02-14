﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XdChatShared.Scheduler {
    public class XdScheduler {
        public static XdScheduler Instance { get; } = new XdScheduler();
        
        private readonly SyncTaskScheduler mainThreadScheduler = new SyncTaskScheduler("MainThread");
        private readonly ConcurrentDictionary<int, Thread> asyncThreads = new ConcurrentDictionary<int, Thread>();

        private XdScheduler() {
        }
        
        public int QueuedSyncTasks => mainThreadScheduler.ScheduledTaskCount;
        public int RunningAsyncTasks => asyncThreads.Count;

        public void RunSync(Action action) {
            Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.mainThreadScheduler);
        }

        public Thread RunAsync(string name, Action action) {
            Thread thread = new Thread(() => {
                action.Invoke();
                asyncThreads.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
            });
            
            asyncThreads[thread.ManagedThreadId] = thread;
            thread.Name = $"AsyncThread-{name}-{thread.ManagedThreadId}-";
            
            thread.Start();
            return thread;
        }
        public static Timeout RunTimeout(Action action, long timeoutTime) {
            Timeout timeout = new Timeout(action, timeoutTime);
            timeout.Start();
            return timeout;
        }
        
        public long CurrentTimeMillis() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public void CheckIsSync() {
            if (mainThreadScheduler.IsMainThread) return;
            throw new InvalidOperationException("Not running on main thread");
        }
    }
}