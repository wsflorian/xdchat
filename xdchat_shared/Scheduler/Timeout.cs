using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XdChatShared.Scheduler;

namespace XdChatShared {
    public class Timeout {
        private Action action;
        private long timeout;
        private long start;
        private Thread thread;

        internal Timeout(Action action, long timeout) {
            this.action = action;
            this.timeout = timeout;
        }

        private void RunThread() {
            while (start + timeout > CurrentTimeMillis()) {
                Thread.Sleep(100);
            }
            if (this.timeout == -1) return; // cancel
            action.Invoke();
        }

        internal void Start() {
            this.start = CurrentTimeMillis();
            this.thread = XdScheduler.Instance.RunAsync("Timeout-Thread", RunThread);
        }

        public void Cancel() {
            this.timeout = -1;
        }

        private static long CurrentTimeMillis() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
