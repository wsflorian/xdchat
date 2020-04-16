using System;
using XdChatShared.Logger;

namespace xdchat_shared.Logger.Impl {
    public class DefaultLogFormatter : ILogFormatter {
        public string Format(XdLogMessage logMessage) {
            return $"{logMessage.TimeStamp:dd.MM.yyyy HH:mm:ss}: {Enum.GetName(typeof(XdLogLevel), logMessage.Level)} -> {logMessage.Message}";
        }
    }
}