using xdchat_server.EventsImpl;
using XdChatShared.Events;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class PongPacketListener : EventListener {
        [EventHandler(Filter = typeof(ClientPacketPong))]
        public void HandlePongPacket(PacketReceivedEvent ev) {
            ev.Client.ReceivePing();
        }
    }
}