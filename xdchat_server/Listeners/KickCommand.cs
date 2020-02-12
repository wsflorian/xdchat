using System.Collections.Generic;
using System.Linq;

namespace xdchat_server.Listeners {
    public class KickCommand : CommandListener {
        public KickCommand() : base("kick") {}

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender != XdServer.Instance.ConsoleCommandSender) {
                sender.SendMessage("No permission");
                return;
            }

            if (args.Count < 1) {
                sender.SendMessage("Usage: kick <nickname> [message]");
                return;
            }

            XdClientConnection client = XdServer.Instance.GetClientByNickname(args[0]);
            if (client == null) {
                sender.SendMessage($"User '{args[0]}' could not be found");
                return;
            }

            if (client == sender) {
                sender.SendMessage("Why would you do this?");
                return;
            }

            client.Disconnect(args.Count < 2
                ? "You have been kicked"
                : JoinArguments(args, 1, args.Count));
        }
    }
}