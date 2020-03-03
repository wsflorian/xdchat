using System;
using System.Linq;
using xdchat_server.Events;

namespace xdchat_server.Commands {
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