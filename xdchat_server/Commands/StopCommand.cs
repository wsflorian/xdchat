﻿using System.Collections.Generic;

namespace xdchat_server.Commands {
    public class StopCommand : Command {
        public StopCommand() : base("stop", "end", "gtfo") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender != XdServer.Instance.ConsoleCommandSender) {
                sender.SendMessage("No permission");
                return;
            }
            
            sender.SendMessage("Stopping server...");
            XdServer.Instance.Stop();
        }
    }
}