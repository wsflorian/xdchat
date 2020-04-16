using System;

namespace XdChatShared.Logger {
    public interface ILogPublisher {
        void Handle(XdLogMessage logMessage);
    }
}