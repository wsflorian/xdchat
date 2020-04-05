using System.Collections.Generic;
using xdchat_server.ClientCon;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class HelpCommand : Command {
        public HelpCommand() : base("help", "List all commands", "?") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            XdServer.Instance.Mod<CommandModule>().Commands.ForEach(command => {
                sender.SendMessage(" /" + command.Name + " - " + command.Description);
            });
        }
    }
}