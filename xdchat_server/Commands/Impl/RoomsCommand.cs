using System.Collections.Generic;
using System.Linq;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class RoomsCommand : Command  {
        public RoomsCommand() : base("rooms", "server.rooms", "List all chatrooms") { }
        protected override void OnCommand(ICommandSender sender, List<string> args) {
            using (XdDatabase db = XdServer.Instance.Db) {
                sender.SendMessage("Available chat rooms:\n" 
                                   + string.Join("\n", db.Rooms.Select(room => " - " + room.Name)));
            }
        }
    }
}