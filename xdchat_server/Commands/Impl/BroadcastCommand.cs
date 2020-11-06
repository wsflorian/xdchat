using System.Collections.Generic;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class BroadcastCommand: Command {
        public BroadcastCommand() : base("broadcast", "server.broadcast","Broadcast a message", "bc", "say") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            string message = $"[Broadcast] {JoinArguments(args, 0, args.Count)}";
            
            XdServer.Instance.Clients.ForEach(client => {
                client.SendMessage(message);
            });
        }
    }
}