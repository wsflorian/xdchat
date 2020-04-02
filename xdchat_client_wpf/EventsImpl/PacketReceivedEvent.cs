using xdchat_client;
using XdChatShared;
using XdChatShared.Events;
using XdChatShared.Packets;

namespace xdchat_client_wpf.EventsImpl
{

    public class PacketReceivedEvent : Event, IEventFilter
    {
        public Packet Packet { get; }

        public PacketReceivedEvent(Packet packet)
        {
            this.Packet = packet;
        }

        public bool DoesMatch(object filter) {
            return this.Packet.GetType().Equals(filter);
        }
    }
}