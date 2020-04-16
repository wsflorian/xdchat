using System;

namespace XdChatShared.Logger {
    public interface ILogFormatter {
        string Format(XdLogMessage logMessage);
    }
}