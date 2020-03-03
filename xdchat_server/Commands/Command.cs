using System.Collections.Generic;
using System.Linq;

namespace xdchat_server.Commands {
    public abstract class Command {
        public string CommandName { get; }
        public List<string> Aliases { get; }
        
        protected Command(string commandName, params string[] aliases) {
            this.CommandName = commandName;
            this.Aliases = new List<string>(aliases);
        }
        
        public abstract void OnCommand(ICommandSender sender, List<string> args);
        
        protected static string JoinArguments(IEnumerable<string> args, int from, int to) {
            return string.Join(' ', args.Take(to).Skip(from));
        }
    }
}