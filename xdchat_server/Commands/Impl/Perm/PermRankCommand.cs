using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl.Perm {
    public class PermRankCommand : Command {
        private const string UsageString = "Correct usage: /perm rank <rank> [<add/remove> <permission>]";

        public PermRankCommand() : base("rank", "server.rank.permissions", "Show all permissions of a rank") { }

        private const string AddArg = "add";
        private const string RemoveArg = "remove";

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (args.Count == 0) {
                sender.SendMessage(UsageString);
                return;
            }

            using (XdDatabase db = XdServer.Instance.Db) {
                DbRank rank = DbRank.GetRank(db, args[0]);

                if (rank == null) {
                    sender.SendMessage($"Rank '{args[0]}' doesn't exist!");
                    return;
                }

                switch (args.Count) {
                    case 1:
                        HandlePermissionList(sender, args, rank);
                        return;

                    case 3:
                        // ReSharper disable once InvertIf
                        if (HandlePermissionChange(sender, args, rank)) {
                            XdDatabase.CachedUserRank.Clear();
                            db.SaveChanges();
                        }
                        return;

                    default:
                        sender.SendMessage(UsageString);
                        return;
                }
            }
        }

        private static bool HandlePermissionChange(ICommandSender sender, List<string> args, DbRank rank) {
            if (!sender.HasPermission("server.rank.modify")) {
                sender.SendMessage("No permission");
                return false;
            }

            if (!Regex.IsMatch(args[2].ToLower(), @"^([a-z]+|\*)(\.([a-z]+|\*))*$")) {
                sender.SendMessage("Invalid permission entered");
                return false;
            }

            DbRankPermission perm = DbRankPermission.All(rank.Id, args[2].ToLower())[0];

            switch (args[1]) {
                case AddArg:
                    AddRankPerm(sender, rank, perm);
                    return true;
                case RemoveArg:
                    RemoveRankPerm(sender, rank, perm);
                    return true;
                default:
                    sender.SendMessage(UsageString);
                    return false;
            }
        }

        private static void AddRankPerm(ICommandSender sender, DbRank rank, DbRankPermission perm) {
            rank.Permissions.Add(perm);
            sender.SendMessage($"Successfully added permission {perm.Permission} to rank {rank.Name}");
        }

        private static void RemoveRankPerm(ICommandSender sender, DbRank rank, DbRankPermission perm) { 
            DbRankPermission dbPerm = rank.Permissions.FirstOrDefault(e => e.Equals(perm));
            if (!rank.Permissions.Contains(dbPerm)) {
                sender.SendMessage($"Permission {perm.Permission} not found on rank {rank.Name}");
                return;
            }

            rank.Permissions.Remove(dbPerm);
            sender.SendMessage($"Successfully removed permission {perm.Permission} of rank {rank.Name}");
        }

        private static void HandlePermissionList(ICommandSender sender, List<string> args, DbRank rank) {
            sender.SendMessage(
                $"The rank '{args[0]}' has the following permissions:\n" +
                string.Join('\n', rank.Permissions.Select(e => " - " + e.Permission)));
        }
    }
}