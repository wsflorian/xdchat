using System.Collections.Generic;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class StopCommand : Command {
        public StopCommand() : base("stop", "server.stop", "Stop the server", "end", "gtfo") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            sender.SendMessage("Stopping server...");
            XdServer.Instance.Stop();
        }
    }
}