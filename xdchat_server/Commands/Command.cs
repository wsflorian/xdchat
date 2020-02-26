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

    public class CommandEvent : Event {
        public ICommandSender Sender { get; }
        public string CommandName { get; }
        public List<string> Args { get; }
        public bool Handled { get; private set; }

        public CommandEvent(ICommandSender sender, string commandMessage) {
            this.Sender = sender;

            List<string> args = new List<string>(commandMessage.Split(" "));

            this.CommandName = args[0];
            if (this.CommandName.StartsWith("/"))
                this.CommandName = this.CommandName.Substring(1);
            
            args.RemoveAt(0);
            this.Args = args;
        }

        public void SetHandled() {
            Handled = true;
        }
    }

    public abstract class CommandListener : EventListener {
        private readonly List<string> aliases;
        private readonly string command;

        protected CommandListener(string command, params string[] aliases) {
            this.command = command;
            this.aliases = new List<string>(aliases);
        }

        [Events.EventHandler]
        public void OnCommandEvent(CommandEvent ev) {
            if (!EqualsIgnoreCase(command, ev.CommandName) 
                && aliases.All(alias => !EqualsIgnoreCase(alias, ev.CommandName))) return;
            
            this.OnCommand(ev.Sender, ev.Args);
            ev.SetHandled();
        }

        protected abstract void OnCommand(ICommandSender sender, List<string> args);

        private static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        protected static string JoinArguments(IEnumerable<string> args, int from, int to) {
            return string.Join(' ', args.Take(to).Skip(from));
        }
    }
}