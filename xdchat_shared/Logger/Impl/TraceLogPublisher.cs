using System.Diagnostics;
using XdChatShared.Logger;

namespace xdchat_shared.Logger.Impl {
    public class TraceLogPublisher : ILogPublisher {
        public void Handle(XdLogMessage logMessage) {
            Trace.WriteLine(logMessage.ToString());
        }
    }
}