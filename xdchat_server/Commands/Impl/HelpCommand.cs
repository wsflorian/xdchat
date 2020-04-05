using System.Collections.Generic;
using xdchat_server.ClientCon;

namespace xdchat_server.Commands {
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