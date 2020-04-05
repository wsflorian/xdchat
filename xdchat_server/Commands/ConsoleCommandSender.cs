using SimpleLogger;

namespace xdchat_server.Commands {
    public class ConsoleCommandSender : ICommandSender {
        public void SendMessage(string text) {
            Logger.Log("> " + text);
        }

        public string GetName() {
            return "[Console]";
        }
    }
}