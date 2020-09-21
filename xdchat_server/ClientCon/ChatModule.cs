using xdchat_server.EventsImpl;
using xdchat_server.Server;
using xdchat_shared.Logger.Impl;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class ChatModule : Module<XdClientConnection>, IEventListener {
        public ChatModule(XdClientConnection context) : base(context, XdServer.Instance) {
        }
        
        [XdEventHandler(typeof(ClientPacketChatMessage), true)]
        public void HandleAuthPacket(PacketReceivedEvent ev) {
            ClientPacketChatMessage packet = (ClientPacketChatMessage) ev.Packet;
            XdLogger.Info($"<{ev.Client.Mod<AuthModule>().Nickname}>: {packet.Text}");
            
            if (packet.Text.StartsWith("/")) {
                XdServer.Instance.Mod<CommandModule>().EmitCommand(ev.Client, packet.Text);
                return;
            }

            XdServer.Instance.Broadcast(new ServerPacketChatMessage {
                HashedUuid = ev.Client.Mod<AuthModule>().HashedUuid,
                Text = packet.Text
            }, con => con != ev.Client 
                      && con.Mod<AuthModule>().DbSession.Room.Id == ev.Client.Mod<AuthModule>().DbSession.Room.Id);
        }
    }
}