using System;
using System.Text;
using JetBrains.Annotations;
using xdchat_server.Server;
using XdChatShared.Scheduler;

namespace xdchat_server {
    static class Program {
        static void Main([UsedImplicitly] string[] args) {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            XdScheduler.QueueSyncTask(() => XdServer.Instance.Start());
        }
    }
}
