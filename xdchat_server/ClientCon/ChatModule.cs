using SimpleLogger;
using xdchat_server.EventsImpl;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class ChatModule : Module<XdClientConnection>, IEventListener {
        public ChatModule(XdClientConnection context) : base(context, XdServer.Instance.EventEmitter) {
        }
        
        [EventHandler(typeof(ClientPacketChatMessage), true)]
        public void HandleAuthPacket(PacketReceivedEvent ev) {
            ClientPacketChatMessage packet = (ClientPacketChatMessage) ev.Packet;
            Logger.Log($"<{ev.Client.Mod<AuthModule>().Nickname}>: {packet.Text}");
            
            if (packet.Text.StartsWith("/")) {
                XdServer.Instance.Mod<CommandModule>().EmitCommand(ev.Client, packet.Text);
                return;
            }

            XdServer.Instance.Broadcast(new ServerPacketChatMessage {
                HashedUuid = ev.Client.Mod<AuthModule>().HashedUuid,
                Text = packet.Text
            }, con => con != ev.Client);
        }
    }
}