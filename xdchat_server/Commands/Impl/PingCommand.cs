using System.Collections.Generic;
using xdchat_server.ClientCon;

namespace xdchat_server.Commands.Impl {
    public class PingCommand : Command {
        public PingCommand() : base("ping", "Show your ping") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender == ConsoleCommandSender) {
                sender.SendMessage("Console's ping is 0. Always.");
                return;
            }

            XdClientConnection connection = (XdClientConnection) sender;
            sender.SendMessage($"Your ping is {connection.Mod<PingModule>().Ping} ms.");
        }
    }
}