using System.Collections.Generic;
using xdchat_server.Commands;

namespace xdchat_server.Events {
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
}