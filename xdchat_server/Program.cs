using System;
using System.Text;
using System.Threading;
using XdChatShared.Scheduler;

namespace xdchat_server {
    static class Program {
        static void Main(string[] args) {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            XdScheduler.Instance.RunSync(() => XdServer.Instance.Start());
        }
    }
}
