using System.Collections.Generic;
using System.Text;

namespace xdchat_server.Commands {
    public class ListCommand : Command {
        public ListCommand() : base("list") { }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            List<XdClientConnection> clients = XdServer.Instance.GetAuthenticatedClients();
            StringBuilder builder = new StringBuilder();

            builder.Append($"{clients.Count} Client(s) connected: ");

            for (int i = 0; i < clients.Count; i++) {
                if (i % 4 == 0)
                    builder.Append("\n");
                
                XdClientConnection client = clients[i];
                builder.Append(client.Auth.Nickname.PadRight(21, ' '));
            }

            sender.SendMessage(builder.ToString());
        }
    }
}