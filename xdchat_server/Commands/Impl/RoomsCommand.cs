using System.Collections.Generic;
using System.Linq;
using xdchat_server.Db;
using xdchat_server.Server;

namespace xdchat_server.Commands.Impl {
    public class RoomsCommand : Command  {
        public RoomsCommand() : base("rooms", "List all chatrooms") { }
        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (!sender.HasPermission("server.rooms")) {
                sender.SendMessage("No permission");
                return;
            }

            using (XdDatabase db = XdServer.Instance.Db) {
                sender.SendMessage("Available chat rooms:\n" 
                                   + string.Join("\n", db.Rooms.Select(room => " - " + room.Name)));
            }
        }
    }
}