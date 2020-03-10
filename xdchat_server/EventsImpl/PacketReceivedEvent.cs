using XdChatShared.Events;
using XdChatShared.Packets;

namespace xdchat_server.EventsImpl {
    public class PacketReceivedEvent : Event, IEventFilter {
        public XdClientConnection Client { get; }
        public Packet Packet { get; }

        public PacketReceivedEvent(XdClientConnection client, Packet packet) {
            this.Client = client;
            this.Packet = packet;
        }

        public bool DoesMatch(object filter) {
            return Packet.GetType().Equals(filter);
        }
    }
}