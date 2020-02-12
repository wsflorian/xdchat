using System.Collections.Generic;

namespace xdchat_server.Listeners {
    public class WhisperCommand : CommandListener {
        public WhisperCommand() : base("whisper", "msg", "w") {
        }

        protected override void OnCommand(ICommandSender sender, List<string> args) {
            if (args.Count < 2) {
                sender.SendMessage("Usage: /whisper <nickname> <message>");
                return;
            }

            XdClientConnection target = XdServer.Instance.GetClientByNickname(args[0]);
            if (target == null) {
                sender.SendMessage($"User '{args[0]}' could not be found");
                return;
            }

            if (target == sender) {
                sender.SendMessage("Why would you use a chat to talk to yourself?");
                return;
            }

            string message = JoinArguments(args, 1, args.Count);
            sender.SendMessage($"Message to {target.GetName()}: {message}");
            target.SendMessage($"Message from {sender.GetName()}: {message}");
        }
    }
}