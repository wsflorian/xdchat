using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace xdchat_server.Commands {
    public abstract class Command {
        [NotNull] public string CommandName { get; }
        public List<string> Aliases { get; }
        
        protected Command([NotNull] string commandName, params string[] aliases) {
            this.CommandName = commandName;
            this.Aliases = new List<string>(aliases);
        }
        
        public abstract void OnCommand([NotNull] ICommandSender sender, [NotNull] List<string> args);
        
        [NotNull] 
        protected static string JoinArguments([NotNull] IEnumerable<string> args, int from, int to) {
            return string.Join(' ', args.Take(to).Skip(from));
        }
    }
}