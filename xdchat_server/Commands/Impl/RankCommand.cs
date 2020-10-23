using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl
{
    public class RankCommand : Command
    {
        private const string UsageString = "Correct usage: /perm rank <rank> [<add/remove> <permission>]";

        public RankCommand() : base("rank", "Shows you all permissions of a rank.")
        { }

        public const string AddArg = "add";
        public const string RemArg = "remove";

        public override void OnCommand(ICommandSender sender, List<string> args)
        {
            if (!sender.HasPermission("server.rank.permissions")) {
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
                var rank = DbRank.GetRank(db, args[0]);

                if (rank == null)
                {
                    sender.SendMessage($"Rank '{args[0]}' doesn't exist!");
                    return;
                }

                switch (args.Count)
                {
                    case 1:
                        HandlePermissionList(sender, args, rank);
                        return;
                    
                    case 3:
                        HandlePermissionChange(sender, args, rank);
                        db.SaveChanges();
                        return;
                    
                    default:
                        sender.SendMessage(UsageString);
                        return;
                }
            }
        }

        private static void HandlePermissionChange(ICommandSender sender, List<string> args, DbRank rank)
        {
            if (!sender.HasPermission("server.rank.modify"))
            {
                sender.SendMessage("No permission");
                return;
            }

            if (!Regex.IsMatch(args[2].ToLower(), @"^([a-z]+|\*)(\.([a-z]+|\*))*$"))
            {
                sender.SendMessage("Invalid permission entered");
                return;
            }

            var perm = DbRankPermission.All(rank.Id, args[2].ToLower())[0];

            switch (args[1])
            {
                case AddArg:
                    rank.Permissions.Add(perm);
                    sender.SendMessage($"Successfully added permission {perm.Permission} to rank {rank.Name}");
                    return;
                case RemArg:
                    sender.SendMessage(RemoveRankPerm(sender, rank, perm) ?
                        $"Successfully removed permission {perm.Permission} of rank {rank.Name}" :
                        $"Permission {perm.Permission} not found on rank {rank.Name}");
                    return;
                default:
                    return;
            }
        }

        private static bool RemoveRankPerm(ICommandSender sender, DbRank rank, DbRankPermission perm)
        {
            var dbPerm = rank.Permissions.FirstOrDefault(e => e.Equals(perm));
            if (!rank.Permissions.Contains(dbPerm)) return false;
            
            rank.Permissions.Remove(dbPerm);
            return true;
        }

        private static void HandlePermissionList(ICommandSender sender, List<string> args, DbRank rank)
        {
            sender.SendMessage(
                $"The rank '{args[0]}' has the following permissions:\n" +
                string.Join('\n', rank.Permissions.Select(e => " - " + e.Permission)));
        }
    }
}