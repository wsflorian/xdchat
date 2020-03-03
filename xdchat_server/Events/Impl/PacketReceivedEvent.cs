using System;
using XdChatShared.Packets;

namespace xdchat_server.Events {
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