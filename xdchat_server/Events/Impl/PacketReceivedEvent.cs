using System;
using XdChatShared.Packets;

namespace xdchat_server.Events {
    public class PacketReceivedEvent : Event, IEventFilter {
        private XdClientConnection client;
        private Packet packet;

        public PacketReceivedEvent(XdClientConnection client, Packet packet) {
            this.client = client;
            this.packet = packet;
        }

        public bool DoesMatch(string filter) {
            return packet.GetType().Name.Equals(filter);
        }
    }
}