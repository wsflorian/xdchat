using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl
{
    public class UserCommand : Command
    {
        private const string UsageString = "Correct usage: /perm user <rank> [<user>]";

        public UserCommand() : base("user", "Shows you the current rank of a user.")
        { }

        public override void OnCommand(ICommandSender sender, List<string> args)
        {
            if (!sender.HasPermission("user.rank.get")) {
                sender.SendMessage("No permission");
                return;
            }

            if (args.Count == 0)
            {
                sender.SendMessage(UsageString);
                return;
            }

            using (XdDatabase db = XdServer.Instance.Db)
            {
                var user = XdServer.Instance.GetClientByNickname(args[0]);

                if (user == null)
                {
                    sender.SendMessage($"User '{args[0]}' is not online!");
                    return;
                }
                
                switch (args.Count)
                {
                    case 1:
                        HandleGetRank(sender, args, user.Auth.DbUser);
                        return;
                    
                    case 2:
                        db.Attach(user.Auth.DbUser);
                        HandleRankChange(); 
                        db.SaveChanges();
                        return;
                    
                    default:
                        sender.SendMessage(UsageString);
                        return;
                }
            }
        }

        private static void HandleGetRank(ICommandSender sender, List<string> args, DbUser user)
        {
            sender.SendMessage($"Rank of user {args[0]} is {user.Rank.Name}");
        }

        private static void HandleRankChange()
        {
            
        }
    }
}