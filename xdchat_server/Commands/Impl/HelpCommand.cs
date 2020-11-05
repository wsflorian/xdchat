using System.Collections.Generic;
using System.Linq;
using System.Text;
using xdchat_server.ClientCon;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class HelpCommand : Command {
        public HelpCommand() : base("help", "server.help", "List all commands", "?") { }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            StringBuilder builder = new StringBuilder();
            XdServer.Instance.Mod<CommandModule>().Commands
                .OrderBy(command => command.Name)
                .ToList()
                .ForEach(command => builder.Append(" /" + command.Name + " - " + command.Description + "\n"));
            
            sender.SendMessage(builder.ToString());
        }
    }
}