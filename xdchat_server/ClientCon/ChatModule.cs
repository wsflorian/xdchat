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
            XdClientConnection client = ev.Client;
            
            using (XdDatabase db = XdServer.Instance.Db) {
                DbUserSession session = client.Auth.GetDbSession(db);
                
                XdLogger.Info($"<{session.Room.Name}/{client.Auth.Nickname}>: {packet.Text}");
            
                if (packet.Text.StartsWith("/")) {
                    XdServer.Instance.Mod<CommandModule>().EmitCommand(client, packet.Text);
                    return;
                }

                db.Attach(session);
                DbMessage.Insert(db, session.Room, session.User, packet.Text);
                db.SaveChanges();

                // ReSharper disable once AccessToDisposedClosure
                XdServer.Instance.Broadcast(new ServerPacketChatMessage {
                    HashedUuid = client.Mod<AuthModule>().HashedUuid,
                    Text = packet.Text
                }, con => con != client && con.Auth.GetDbSession(db).Room.Id == session.Room.Id);
            }
        }

        [XdEventHandler(null, true)]
        public void HandleClientReady(ClientReadyEvent ev) {
            using (XdDatabase db = XdServer.Instance.Db) {
                DbUserSession session = ev.Client.Auth.GetDbSession(db);

                ev.Client.ClearChat();

                DbMessage.GetRecent(db, session.Room.Id, 20).ForEach(msg => {
                    ev.Client.SendOldMessage(Helper.Sha256Hash(msg.User.Uuid), msg.TimeStamp, msg.Content);
                }); // send to client
                
                ev.Client.SendMessage("Your current chatroom is: " + session.Room.Name);
            }
        }
    }
}