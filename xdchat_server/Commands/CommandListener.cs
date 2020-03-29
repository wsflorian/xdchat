using System.Linq;
using xdchat_server.EventsImpl;
using XdChatShared.Events;

namespace xdchat_server.Commands {
    public class CommandListener : IEventListener {
        private readonly Command command;

        public CommandListener(Command command) {
            this.command = command;
        }

        [EventHandler]
        public void OnCommandEvent(CommandEvent ev) {
            if (!EqualsIgnoreCase(command.CommandName, ev.CommandName) 
                && command.Aliases.All(alias => !EqualsIgnoreCase(alias, ev.CommandName))) return;
            
            command.OnCommand(ev.Sender, ev.Args);
            ev.SetHandled();
        }

        private static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, System.StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}