using xdchat_shared.Logger.Impl;
using XdChatShared.Logger;

namespace xdchat_server.Commands {
    public class ConsoleCommandSender : ICommandSender {
        public void SendMessage(string text) {
            XdLogger.Info("> " + text);
        }

        public string GetName() {
            return "[Console]";
        }
    }
}