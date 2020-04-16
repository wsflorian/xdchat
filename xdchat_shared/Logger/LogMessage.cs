using System;

namespace XdChatShared.Logger {
    public class XdLogMessage {
        public Logger Logger { get; }
        public DateTime TimeStamp { get; } = DateTime.Now;
        public XdLogLevel Level { get; }
        public string Message { get; }

        public XdLogMessage(Logger logger, XdLogLevel level, string message) {
            Logger = logger;
            Level = level;
            Message = message;
        }

        public override string ToString() {
            return Logger.Formatter?.Format(this);
        }
    }
}