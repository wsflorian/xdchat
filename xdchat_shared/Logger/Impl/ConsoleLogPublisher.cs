using System;
using XdChatShared.Logger;

namespace xdchat_shared.Logger.Impl {
    public class ConsoleLogPublisher : ILogPublisher {
        public void Handle(XdLogMessage logMessage) {
            Console.WriteLine(logMessage.ToString());
        }
    }
}