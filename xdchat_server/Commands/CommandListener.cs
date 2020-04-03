using System.Linq;
using xdchat_server.EventsImpl;
using XdChatShared.Events;

namespace xdchat_server.Commands {
    public class CommandListener : IEventListener {
        private readonly Command _command;

        public CommandListener(Command command) {
            this._command = command;
        }

        [EventHandler]
        public void OnCommandEvent(CommandEvent ev) {
            if (!EqualsIgnoreCase(_command.CommandName, ev.CommandName) 
                && _command.Aliases.All(alias => !EqualsIgnoreCase(alias, ev.CommandName))) return;
            
            _command.OnCommand(ev.Sender, ev.Args);
            ev.SetHandled();
        }

        private static bool EqualsIgnoreCase(string a, string b) {
            return string.Compare(a, b, System.StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}