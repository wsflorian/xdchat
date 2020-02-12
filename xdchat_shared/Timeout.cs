using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XdChatShared {
    public class TimeoutThread {
        private Action action;
        private Thread thread;
        private long timeout;
        private long start;

        private TimeoutThread(Action action, long timeout) {
            this.action = action;
            this.timeout = timeout;
            this.thread = new Thread(RunThread);
        }

        private void RunThread() {
            while (start + timeout > CurrentTimeMillis()) {
                Thread.Sleep(100);
            }
            if (this.timeout == -1) return; // cancel
            action.Invoke();
        }

        public void Start() {
            this.start = CurrentTimeMillis();
            this.thread.Start();
        }

        public void Cancel() {
            this.timeout = -1;
        }

        private static long CurrentTimeMillis() {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static TimeoutThread SetTimeout(Action action, long timeoutTime) {
            TimeoutThread timeout = new TimeoutThread(action, timeoutTime);
            timeout.Start();
            return timeout;
        }
    }
}
