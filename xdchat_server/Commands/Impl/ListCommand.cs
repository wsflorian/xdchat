using System.Collections.Generic;
using System.Text;
using xdchat_server.ClientCon;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class ListCommand : Command {
        public ListCommand() : base("list", "server.list","Show a list of all connected users") { }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            List<XdClientConnection> clients = XdServer.Instance.GetAuthenticatedClients();
            StringBuilder builder = new StringBuilder();

            builder.Append($"{clients.Count} Client(s) connected: ");

            for (int i = 0; i < clients.Count; i++) {
                if (i % 4 == 0)
                    builder.Append("\n");
                
                XdClientConnection client = clients[i];
                builder.Append(client.Mod<AuthModule>().Nickname.PadRight(21, ' '));
            }

            sender.SendMessage(builder.ToString());
        }
    }
}