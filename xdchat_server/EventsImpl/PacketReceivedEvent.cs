using xdchat_server.ClientCon;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_server.EventsImpl {
    public class PacketReceivedEvent : Event, IEventFilter, IAnonymousContextProvider {
        public XdClientConnection Client { get; }
        public Packet Packet { get; }

        public PacketReceivedEvent(XdClientConnection client, Packet packet) {
            this.Client = client;
            this.Packet = packet;
        }

        public bool DoesMatch(object filter) {
            return Packet.GetType().Equals(filter);
        }

        public dynamic AnonymousModuleContext => Client;
    }
}