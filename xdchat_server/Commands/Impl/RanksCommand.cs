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
            
            StringBuilder builder = new StringBuilder();
            builder.Append($"List of ranks:");

            using (XdDatabase db = XdServer.Instance.Db) {
                foreach (var rank in db.Ranks)
                {
                    builder.Append($"\n - {rank.Name}");
                }

                db.SaveChanges();
            }

            sender.SendMessage(builder.ToString());
        }

        public string Uuid { get; set; }
    }
}