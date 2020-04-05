using System.Collections.Generic;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class SayCommand: Command {
        public SayCommand() : base("broadcast", "Broadcast a message", "bc", "say") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender != ConsoleCommandSender) {
                sender.SendMessage("No permission");
                return;
            }
            
            string message = $"[Broadcast] {JoinArguments(args, 0, args.Count)}";
            
            sender.SendMessage(message);
            XdServer.Instance.Clients.ForEach(client => {
                client.SendMessage(message);
            });
        }
    }
}