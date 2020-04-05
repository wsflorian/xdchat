using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using SimpleLogger;
using Timer = System.Timers.Timer;

/* => Threading System <=
 * 
 * Main Thread
 * => Executes only one task at a time
 * => Used for everything everything regarding Client, Server or Connections to avoid any race conditions
 *
 * Async Thread Pool
 * => Executes many tasks in parallel
 * => Used for blocking / calculation-intensive operations
 *
 * WPF UI Thread (provided by WPF framework)
 * => Executes only one task at a time
 * => Used for (some) UI operations
 */
namespace XdChatShared.Scheduler {
    public static class XdScheduler {
        private static readonly SyncTaskScheduler MainThreadScheduler = new SyncTaskScheduler("MainThread");
        private static readonly TaskScheduler AsyncTaskScheduler = TaskScheduler.Default;

        /* External functions with Action */
        public static Task QueueSyncTask(Action action) => QueueTask(VoidToFunc(action), MainThreadScheduler);
        
        public static Task QueueAsyncTask(Action action, bool longRunning = false) 
            => QueueAsyncTask(VoidToFunc(action), longRunning);

        public static Timer QueueSyncTaskScheduled(Action func, long millis, bool repeating = false)
            => QueueSyncTaskScheduled(VoidToFunc(func), millis, repeating);
        public static Timer QueueAsyncTaskScheduled(Action func, long millis, bool repeating = false)
            => QueueAsyncTaskScheduled(VoidToFunc(func), millis, repeating);

        /* External functions with Func<Task> */
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
        private static Func<Task> VoidToFunc(Action action) {
            if (IsActionAsync(action))
                throw new InvalidOperationException("Action must be synchronous");
            
            return async () => { action.Invoke(); };
        }
#pragma warning restore 1998
        
        private static bool IsActionAsync(Action action) {
            return action.Method.IsDefined(typeof(AsyncStateMachineAttribute), false);
        }
        
        public static void CheckIsMainThread() {
            if (MainThreadScheduler.IsMainThread) return;
            throw new InvalidOperationException("Not running on main thread");
        }

        public static void CheckIsNotMainThread() {
            if (!MainThreadScheduler.IsMainThread) return;
            throw new InvalidOperationException("Running on main thread");
        }
    }
}