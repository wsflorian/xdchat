using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using xdchat_server.ClientCon;

namespace xdchat_server.Commands {
    public abstract class Command {
        protected static ConsoleCommandSender ConsoleCommandSender => XdServer.Instance.Mod<CommandModule>().ConsoleCommandSender;
        
        public string Name { get; }
        public string Description { get; }
        public List<string> AlternateNames { get; }

        protected Command([NotNull] string name, [NotNull] string description, params string[] aliases) {
            this.Name = name;
            this.Description = description;
            this.AlternateNames = new List<string>(aliases);
        }

        public bool Matches(string name) {
            return EqualsIgnoreCase(Name, name) 
                   || AlternateNames.Any(alternateName => EqualsIgnoreCase(alternateName, name));
        }
        
        public abstract void OnCommand([NotNull] ICommandSender sender, [NotNull] List<string> args);
        
        [NotNull] 
        protected static string JoinArguments([NotNull] IEnumerable<string> args, int from, int to) {
            return string.Join(' ', args.Take(to).Skip(from));
        }
        
        private static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, System.StringComparison.OrdinalIgnoreCase) == 0;
        } 
    }
}