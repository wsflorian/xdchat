using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xdchat_server.ClientCon;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl
{
    public class RanksCommand : Command
    {
        public RanksCommand() : base("ranks", "Shows all ranks") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (!sender.HasPermission("server.rank.list")) {
                sender.SendMessage("No permission");
                return;
            }
            
            using (XdDatabase db = XdServer.Instance.Db)
            {
                sender.SendMessage(
                    "List of ranks:" + 
                    string.Join("\n", db.Ranks.Select(rank => " - " + rank.Name)));
            }
            
        }

        public string Uuid { get; set; }
    }
}