using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using xdchat_server.ClientCon;
using xdchat_server.Server;
using XdChatShared.Misc;

namespace xdchat_server.Commands {
    /* => Command system <=
     *
     * General
     * => Use the help command to get a list of all commands
     * => Every user message starting with a / is a command
     * => Every message from the console is a command
     * => Both commands and aliases can be used to execute a command
     * => ConsoleCommandSender represents the console when inputting commands
     */
    public abstract class Command {
        protected static ConsoleCommandSender ConsoleCommandSender => XdServer.Instance.Mod<CommandModule>().ConsoleCommandSender;
        
        public string Name { get; }
        public string Permission { get; }
        public string Description { get; }
        public List<string> AlternateNames { get; }

        protected Command([NotNull] string name, [NotNull] string permission, [NotNull] string description, params string[] aliases) {
            this.Name = name;
            this.Permission = permission;
            this.Description = description;
            this.AlternateNames = new List<string>(aliases);
        }

        public bool Matches(string name) {
            return Helper.EqualsIgnoreCase(Name, name) 
                   || AlternateNames.Any(alternateName => Helper.EqualsIgnoreCase(alternateName, name));
        }

        public void Invoke([NotNull] ICommandSender sender, [NotNull] List<string> args) {
            if (!sender.HasPermission(this.Permission)) {
                sender.SendMessage("No permission");
                return;
            }
            
            this.OnCommand(sender, args);
        }
        
        protected abstract void OnCommand([NotNull] ICommandSender sender, [NotNull] List<string> args);
        
        [NotNull] 
        protected static string JoinArguments([NotNull] IEnumerable<string> args, int from, int to) {
            return string.Join(' ', args.Take(to).Skip(from));
        }
    }
}