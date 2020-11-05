using System.Collections.Generic;
using xdchat_server.ClientCon;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl.Perm {
    public class PermUserCommand : Command {
        private const string UsageString = "Correct usage: /perm user <user> <rank>";

        public PermUserCommand() : base("user", "user.rank.get", "Shows the current rank of a user") { }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (args.Count == 0) {
                sender.SendMessage(UsageString);
                return;
            }

            using (XdDatabase db = XdServer.Instance.Db) {
                XdClientConnection user = XdServer.Instance.GetClientByNickname(args[0]);

                if (user == null) {
                    sender.SendMessage($"User '{args[0]}' is not online!");
                    return;
                }

                DbUser dbUser = user.Auth.GetDbUser(db);
                switch (args.Count) {
                    case 1:
                        HandleGetRank(sender, args, dbUser);
                        return;

                    case 2:
                        HandleRankChange(db, sender, args, dbUser);
                        XdDatabase.CachedUserRank.Clear();
                        db.SaveChanges();
                        return;

                    default:
                        sender.SendMessage(UsageString);
                        return;
                }
            }
        }

        private static void HandleGetRank(ICommandSender sender, List<string> args, DbUser user) {
            sender.SendMessage($"Rank of user {args[0]} is {user.Rank.Name}");
        }

        private static void HandleRankChange(XdDatabase db, ICommandSender sender, List<string> args, DbUser user) {
            DbRank rank = DbRank.GetRank(db, args[1]);
            if (rank == null) {
                sender.SendMessage($"Rank '{args[1]}' not found");
                return;
            }
            
            user.Rank = rank;
            DbUser.Update(db, user);
            sender.SendMessage($"Rank of user {args[0]} was changed to {user.Rank.Name}");
        }
    }
}