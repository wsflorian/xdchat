using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace xdchat_server.Commands.Impl
{
    public class RankHandlerCommand : Command
    {
        private Dictionary<string, Command> Commands = new Dictionary<string, Command>()
        {
            { "ranks", new RanksCommand() },
            { "rank", new RankCommand() },
            { "user", new UserCommand() },
            // {"help", new PermHelpCommand()}
        };
        
        public RankHandlerCommand() : base("perm", "server.perm", "Everything about ranks try /perm help") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args)
        {
            if (args.Count == 0)
            {
                sender.SendMessage("Try /perm help for information on how to use the command.");
                return;
            }

            if (Commands.TryGetValue(args[0], out var command))
            {
                command.Invoke(sender, args.Skip(1).ToList());
            }
            else
            {
                sender.SendMessage($"{args[0]} not found in /perm command");
            }
        }
    }
}