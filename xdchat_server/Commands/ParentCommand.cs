using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace xdchat_server.Commands {
    public class ParentCommand : Command {
        private readonly List<Command> _children = new List<Command>();
        
        public ParentCommand([NotNull] string name, [NotNull] string permission, [NotNull] string description, params string[] aliases) 
            : base(name, permission, description, aliases) {}
        
        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (args.Count < 1) {
                sender.SendMessage(GetSubCommandsMessage());
                return;
            }

            Command command = _children.FirstOrDefault(cmd => cmd.Matches(args[0]));
            if (command == null) {
                sender.SendMessage("Sub-Command not found\n" + GetSubCommandsMessage());
                return;
            }

            command.Invoke(sender, args.Skip(1).ToList());
        }

        private string GetSubCommandsMessage() {
            string commandNames = string.Join(", ", _children.Select(child => child.Name));
            return $"Available sub-commands: {commandNames}";
        }

        protected void AddChildCommand(Command command) => _children.Add(command);
    }
}