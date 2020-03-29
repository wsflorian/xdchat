using SimpleLogger;
using xdchat_server.EventsImpl;
using XdChatShared.Events;
using XdChatShared.Packets;

namespace xdchat_server.ClientCon {
    public class AuthPacketListener : IEventListener {
        [EventHandler(Filter = typeof(ClientPacketAuth))]
        public void HandleAuthPacket(PacketReceivedEvent ev) {
            ClientPacketAuth packet = (ClientPacketAuth) ev.Packet;
            
            if (XdServer.Instance.GetClientByNickname(packet.Nickname) != null) {
                ev.Client.Disconnect("This nickname is already used");
                return;
            }

            if (XdServer.Instance.GetClientByUuid(packet.Uuid) != null) {
                ev.Client.Disconnect("You are already connected");
                return;
            }

            Authentication auth = ev.Client.Auth = new Authentication(packet.Nickname, packet.Uuid);
            
            XdServer.Instance.SendUserListUpdate(ev.Client);
            Logger.Log($"Client authenticated: {auth.Nickname} ({auth.Uuid})");
        }
    }
}