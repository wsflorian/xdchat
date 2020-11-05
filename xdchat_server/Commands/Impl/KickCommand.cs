using System.Collections.Generic;
using xdchat_server.ClientCon;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class KickCommand : Command {
        public KickCommand() : base("kick", "user.kick","Kick a connected user") {}

        protected override void OnCommand(ICommandSender sender, List<string> args) {
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