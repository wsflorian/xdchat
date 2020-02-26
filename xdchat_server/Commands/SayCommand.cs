using System.Collections.Generic;

namespace xdchat_server.Commands {
    public class SayCommand: CommandListener {
        public SayCommand() : base("broadcast", "bc", "say") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender != XdServer.Instance.ConsoleCommandSender) {
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