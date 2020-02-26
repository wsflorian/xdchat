using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLogger;
using xdchat_server.Events;

namespace xdchat_server.Commands {
    public interface ICommandSender {
        void SendMessage(string text);
        string GetName();
    }

    public class ConsoleCommandSender : ICommandSender {
        public void SendMessage(string text) {
            Logger.Log("> " + text);
        }

        public string GetName() {
            return "#CONSOLE";
        }
    }

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

    public class CommandListener : EventListener {
        private readonly Command command;

        public CommandListener(Command command) {
            this.command = command;
        }

        [Events.EventHandler]
        public void OnCommandEvent(CommandEvent ev) {
            if (!EqualsIgnoreCase(command.CommandName, ev.CommandName) 
                && command.Aliases.All(alias => !EqualsIgnoreCase(alias, ev.CommandName))) return;
            
            command.OnCommand(ev.Sender, ev.Args);
            ev.SetHandled();
        }

        private static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}