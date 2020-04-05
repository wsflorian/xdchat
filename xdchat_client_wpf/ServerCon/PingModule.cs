using System.Collections.Generic;
using System.Diagnostics;
using xdchat_client_wpf.EventsImpl;
using XdChatShared.Events;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_client_wpf.ServerCon
{
    public class PingModule : Module<XdServerConnection>, IEventListener
    {
        public List<string> Messages;
        
        public PingModule(XdServerConnection context) : base(context, XdClient.Instance)
        {
        }

        [EventHandler(typeof(ServerPacketPing))]
        public void HandlePacketEvent(PacketReceivedEvent evt) {
           //  XdClient.Instance.Connection
           Trace.WriteLine("responding with pong");
           this.Context.Send(new ClientPacketPong());
        }
    }
}