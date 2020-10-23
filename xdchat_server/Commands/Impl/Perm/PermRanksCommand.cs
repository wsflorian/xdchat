using System.Collections.Generic;
using System.Linq;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl.Perm
{
    public class PermRanksCommand : Command
    {
        public PermRanksCommand() : base("ranks", "server.rank.list","Shows all ranks") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) { 
            using (XdDatabase db = XdServer.Instance.Db) {
                sender.SendMessage(
                    "List of ranks:" + 
                    string.Join("\n", db.Ranks.Select(rank => " - " + rank.Name)));
            }
        }
    }
}