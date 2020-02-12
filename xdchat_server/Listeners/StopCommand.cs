using System.Collections.Generic;

namespace xdchat_server.Listeners {
    public class StopCommand : CommandListener {
        public StopCommand() : base("stop", "end", "gtfo") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender != XdServer.Instance.ConsoleCommandSender) {
                sender.SendMessage("No permission");
                return;
            }
            
            sender.SendMessage("Stopping server...");
            XdServer.Instance.Stop();
        }
    }
}