using SimpleLogger.Logging;
using SimpleLogger.Logging.Formatters;
using XdChatShared.Misc;

namespace xdchat_server {
    class ServerLoggerHandler : ILoggerHandler {
        private readonly ILoggerFormatter _formatter = new XdLoggerFormatter();
        private readonly ConsoleHandler _handler;

        public ServerLoggerHandler(ConsoleHandler handler) {
            this._handler = handler;
        }

        public void Publish(LogMessage logMessage) {
            _handler.WriteLine(_formatter.ApplyFormat(logMessage));
        }
    }
}