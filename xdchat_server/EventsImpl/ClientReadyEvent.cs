using xdchat_server.ClientCon;
using XdChatShared.Events;
using XdChatShared.Modules;

namespace xdchat_server.EventsImpl {
    public class ClientReadyEvent : Event, IAnonymousContextProvider  {
        public XdClientConnection Client { get; }
        
        public ClientReadyEvent(XdClientConnection client) {
            this.Client = client;
        }
        
        public dynamic AnonymousModuleContext => Client;
    }
}