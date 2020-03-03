using SimpleLogger;
using xdchat_server.Events;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class ChatPacketListener : EventListener {
        [EventHandler(Filter = typeof(ClientPacketChatMessage))]
        public void HandleAuthPacket(PacketReceivedEvent ev) {
            ClientPacketChatMessage packet = (ClientPacketChatMessage) ev.Packet;
            Logger.Log($"<{ev.Client.Auth?.Nickname}>: {packet.Text}");
            
            if (packet.Text.StartsWith("/")) {
                XdServer.Instance.EmitCommand(ev.Client, packet.Text);
                return;
            }

            XdServer.Instance.Broadcast(new ServerPacketChatMessage {
                HashedUuid = ev.Client.Auth?.HashedUuid,
                Text = packet.Text
            }, con => con != ev.Client);
        }
    }
}