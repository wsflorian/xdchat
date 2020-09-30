using xdchat_server.Db;
using xdchat_server.EventsImpl;
using xdchat_server.Server;
using xdchat_shared.Logger.Impl;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class ChatModule : Module<XdClientConnection>, IEventListener {
        public ChatModule(XdClientConnection context) : base(context, XdServer.Instance) {
        }
        
        [XdEventHandler(typeof(ClientPacketChatMessage), true)]
        public void HandleChatMessage(PacketReceivedEvent ev) {
            ClientPacketChatMessage packet = (ClientPacketChatMessage) ev.Packet;
            XdLogger.Info($"<{ev.Client.Mod<AuthModule>().Nickname}>: {packet.Text}");
            
            if (packet.Text.StartsWith("/")) {
                XdServer.Instance.Mod<CommandModule>().EmitCommand(ev.Client, packet.Text);
                return;
            }

            DbUserSession session = ev.Client.Auth.DbSession;
            using (XdDatabase db = XdServer.Instance.Db) {
                db.Attach(session);
                DbMessage.Insert(db, session.Room, session.User, packet.Text);
                db.SaveChanges();
            }
            
            XdServer.Instance.Broadcast(new ServerPacketChatMessage {
                HashedUuid = ev.Client.Mod<AuthModule>().HashedUuid,
                Text = packet.Text
            }, con => con != ev.Client && con.Auth.DbSession.Room.Id == session.Room.Id);
        }

        [XdEventHandler(null, true)]
        public void HandleClientReady(ClientReadyEvent ev) {
            DbUserSession session = ev.Client.Auth.DbSession;
            
            using (XdDatabase db = XdServer.Instance.Db) {
                DbMessage.GetRecent(db, session.Room.Id, 20).ForEach(msg => {
                    ev.Client.Send(new ServerPacketChatMessage {
                        HashedUuid = Helper.Sha256Hash(msg.User.Uuid),
                        Text = msg.Content
                    });
                }); // send to client
            }
            
            ev.Client.SendMessage("Your current chatroom is: " + session.Room.Name);
        }
    }
}