using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using xdchat_server.Commands;
using xdchat_server.Commands.Impl;
using xdchat_server.Commands.Impl.Perm;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using XdChatShared.Events;
using XdChatShared.Modules;

namespace xdchat_server.ClientCon {
    public class CommandModule : Module<XdServer>, IEventListener {
        public ConsoleCommandSender ConsoleCommandSender { get; } = new ConsoleCommandSender();

        public List<Command> Commands { get; } = new List<Command>();

        public CommandModule(XdServer context) : base(context, XdServer.Instance) {
            Commands.Add(new KickCommand());
            Commands.Add(new ListCommand());
            Commands.Add(new WhisperCommand());
            Commands.Add(new StopCommand());
            Commands.Add(new BroadcastCommand());
            Commands.Add(new PingCommand());
            Commands.Add(new HelpCommand());
            Commands.Add(new RankHandlerCommand());
            Commands.Add(new JoinCommand());
            Commands.Add(new RoomsCommand());
            Commands.Add(new WebAdminCommand());
        }

        public void EmitCommand([NotNull] ICommandSender sender, [NotNull] string commandText) {
            List<string> args = new List<string>(commandText.Split(" "));
            args.RemoveAll(s => s.Length == 0);
                
            string commandName = args[0];
            if (commandName.StartsWith("/"))
                commandName = commandName.Substring(1);

            args.RemoveAt(0);

            Command command = Commands
                .FindAll(cmd => cmd.Matches(commandName))
                .FirstOrDefault();

            if (command == null) {
                sender.SendMessage("Command not found. Use /help for a list of all commands");
                return;
            }

            command.Invoke(sender, args);
        }

        [XdEventHandler]
        public void OnConsoleInput(ConsoleInputEvent ev) {
            EmitCommand(ConsoleCommandSender, ev.Input);
        }
    }
}
