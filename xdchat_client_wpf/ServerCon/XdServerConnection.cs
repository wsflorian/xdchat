using System;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using xdchat_client_wpf;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Connection;
using XdChatShared.Modules;
using XdChatShared.Packets;

namespace xdchat_client {
    public class XdServerConnection : XdConnection, IExtendable<XdServerConnection> {
        private ModuleHolder<XdServerConnection> _moduleHolder;

        public XdServerConnection() {
            _moduleHolder = new ModuleHolder<XdServerConnection>(this);
        }

        public override void Initialize(TcpClient client) {
            base.Initialize(client);

            _moduleHolder.RegisterModule<PingModule>();
            SendAuth(XdClient.Instance.Nickname, XdClient.Instance.Uuid);
        }

        private void SendAuth(string nickname, string uuid) {
            base.Send(new ClientPacketAuth {
                Nickname = nickname, Uuid = uuid
            });
        }

        public void SendMessage(string message) {
            base.Send(new ClientPacketChatMessage {
                Text = message
            });
        }

        protected override void OnPacketReceived(Packet packet) {
            XdClient.Instance.EventEmitter.Emit(new PacketReceivedEvent(packet));
        }

        protected override void OnDisconnect(Exception ex) {
            XdClient.Instance.UpdateStatus(XdConnectionStatus.NOT_CONNECTED, "Connection closed", ex);
        }

        public TModule Mod<TModule>() where TModule : Module<XdServerConnection> {
            return _moduleHolder.Mod<TModule>();
        }
    }
}