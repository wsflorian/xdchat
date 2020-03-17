using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using SimpleLogger;
using Timer = System.Timers.Timer;

namespace XdChatShared.Scheduler {
    public static class XdScheduler {
        //public static XdScheduler Instance { get; } = new XdScheduler();
        
        private static readonly SyncTaskScheduler MainThreadScheduler = new SyncTaskScheduler("MainThread");
        private static readonly TaskScheduler AsyncTaskScheduler = TaskScheduler.Default;
        private static readonly ConcurrentDictionary<int, Thread> AsyncThreads = new ConcurrentDictionary<int, Thread>();

        public static int QueuedSyncTasks => MainThreadScheduler.ScheduledTaskCount;
        public static int RunningAsyncTasks => AsyncThreads.Count;
        
        /* External functions with Action */
        public static Task QueueSyncTask(Action action) => QueueSyncTask(VoidToFunc(action));
        public static Task QueueAsyncTask(Action action, bool longRunning = false) 
            => QueueAsyncTask(VoidToFunc(action), longRunning);

        public static Timer QueueSyncTaskScheduled(Action func, long millis, bool repeating = false)
            => QueueSyncTaskScheduled(VoidToFunc(func), millis, repeating);
        public static Timer QueueAsyncTaskScheduled(Action func, long millis, bool repeating = false)
            => QueueAsyncTaskScheduled(VoidToFunc(func), millis, repeating);

        /* External functions with Func<Task> */
        public static Task QueueSyncTask(Func<Task> func) => QueueTask(func, MainThreadScheduler);
        public static Task QueueAsyncTask(Func<Task> func, bool longRunning = false) => 
            QueueTask(func, AsyncTaskScheduler, longRunning);
        
        public static Timer QueueSyncTaskScheduled(Func<Task> func, long millis, bool repeating = false) 
            => QueueTaskScheduled(func, MainThreadScheduler, millis, repeating);
        public static Timer QueueAsyncTaskScheduled(Func<Task> func, long millis, bool repeating = false) 
            => QueueTaskScheduled(func, AsyncTaskScheduler, millis, repeating);
        
        /* Internal functions */
        private static Task QueueTask(Func<Task> func, TaskScheduler scheduler, bool longRunning = false) {
            return Task.Factory.StartNew(async () => {
                try {
                    await func.Invoke();
                } catch (Exception e) {
                    Logger.Log($"Uncaught exception in task: {e}");
                }
            }, CancellationToken.None, longRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None, scheduler);
        }

        private static Timer QueueTaskScheduled(Func<Task> func, TaskScheduler scheduler, long millis, bool repeating) {
            Timer timer = new Timer {
                Interval = millis, 
                SynchronizingObject = null,
                AutoReset = repeating
            };
            timer.Elapsed += (sender, args) => QueueTask(func, scheduler);
            timer.Start();
            
            return timer;
        }
        
#pragma warning disable 1998
        public static Func<Task> VoidToFunc(Action action) => async () => { action.Invoke(); };
#pragma warning restore 1998
        
        public static Thread RunAsyncThread(string name, Action action) {
            Thread thread = new Thread(() => {
                action.Invoke();
                AsyncThreads.TryRemove(Thread.CurrentThread.ManagedThreadId, out _);
            });

            AsyncThreads[thread.ManagedThreadId] = thread;
            thread.Name = $"AsyncThread-{name}-{thread.ManagedThreadId}-";
            
            thread.Start();
            return thread;
        }
        
        public static long CurrentTimeMillis() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        
        public static void CheckIsSync() {
            if (MainThreadScheduler.IsMainThread) return;
            throw new InvalidOperationException("Not running on main thread");
        }
    }
}