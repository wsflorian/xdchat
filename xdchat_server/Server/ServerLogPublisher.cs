using XdChatShared.Logger;

namespace xdchat_server.Server {
    class ServerLogPublisher : ILogPublisher {
        private readonly ConsoleHandler _handler;

        public ServerLogPublisher(ConsoleHandler handler) {
            this._handler = handler;
        }
        
        public void Handle(XdLogMessage logMessage) {
            _handler.WriteLine(logMessage.ToString());
        }
    }
}