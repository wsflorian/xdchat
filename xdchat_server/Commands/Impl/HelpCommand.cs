using System.Collections.Generic;
using System.Text;
using xdchat_server.ClientCon;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class HelpCommand : Command {
        public HelpCommand() : base("help", "List all commands", "?") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            StringBuilder builder = new StringBuilder();
            XdServer.Instance.Mod<CommandModule>().Commands.ForEach(command => {
                builder.Append(" /" + command.Name + " - " + command.Description + "\n");
            });
            sender.SendMessage(builder.ToString());
        }
    }
}