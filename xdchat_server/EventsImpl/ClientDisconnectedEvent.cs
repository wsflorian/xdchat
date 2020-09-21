using xdchat_server.ClientCon;
using XdChatShared.Events;
using XdChatShared.Modules;

namespace xdchat_server.EventsImpl {
    public class ClientDisconnectedEvent : Event, IAnonymousContextProvider {
        public XdClientConnection Client { get; }

        public ClientDisconnectedEvent(XdClientConnection client) {
            this.Client = client;
        }
        public dynamic AnonymousModuleContext => Client;
    }
}