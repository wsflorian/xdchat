using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using xdchat_server.ClientCon;
using xdchat_server.Db;
using xdchat_server.Server;
using XdChatShared.Misc;
using XdChatShared.Packets;

namespace xdchat_server.Commands.Impl {
    public class JoinCommand : Command {
        public JoinCommand() : base("join", "Join a chatroom", "j") {
        }

        public override void OnCommand(ICommandSender sender, List<string> args) {
            if (sender is ConsoleCommandSender) {
                sender.SendMessage("The console is already in every room");
                return;
            }
            
            if (!sender.HasPermission("user.join")) {
                sender.SendMessage("No permission");
                return;
            }

            if (args.Count < 1) {
                sender.SendMessage("Usage: join <room>");
                return;
            }
            
            using (XdDatabase db = XdServer.Instance.Db) {
                string roomName = args[0];
                DbRoom room = db.Rooms.FirstOrDefault(x => EF.Functions.Like(x.Name, roomName));
                if (room == null) {
                    sender.SendMessage("The chatroom '" + roomName + "' doesn't exist");
                    sender.SendMessage("You can use /rooms for a list of all available rooms");
                    return;
                }
                
                XdClientConnection client = (XdClientConnection) sender;
                if (room.Id == client.Mod<AuthModule>().DbSession.Room.Id) {
                    sender.SendMessage("You are already in this chatroom");
                    return;
                }
                
                client.Mod<AuthModule>().DbSession.Room = room;
                db.Sessions.Update(client.Mod<AuthModule>().DbSession);
                db.SaveChanges();
                
                client.ClearChat();
                
                DbMessage.GetRecent(db, room.Id, 10).ForEach(msg => {
                    client.SendOldMessage(Helper.Sha256Hash(msg.User.Uuid), msg.TimeStamp, msg.Content);
                });
                
                sender.SendMessage("You are now in the chatroom: " + room.Name);
            }
        }
    }
}