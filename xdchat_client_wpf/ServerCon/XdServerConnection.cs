using Microsoft.Win32;
using System.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using xdchat_client_wpf;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.ServerCon;
using XdChatShared;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client {
    public class XdServerConnection : XdConnection {

        public XdServerConnection() {
            XdScheduler.CheckIsMainThread();
        }

        public override void Initialize(TcpClient client) {
            base.Initialize(client);
            SendAuth(XdClient.Instance.Nickname, XdClient.Instance.Uuid);
        }

        private void SendAuth(string nickname, string uuid) {
            base.Send(new ClientPacketAuth {
                Nickname = nickname, Uuid = uuid
            });
        }
        
        public void SendMessage(string message){
            base.Send(new ClientPacketChatMessage {
                Text = message
            });
        }

        protected override void OnPacketReceived(Packet packet) {
            Trace.WriteLine($"Data received... {Packet.ToJson(packet)}");

            if (packet is ServerPacketPing) { // test code
                base.Send(new ClientPacketPong());
            }
        }

        protected override void OnDisconnect(Exception ex) {
            XdClient.Instance.UpdateStatus(XdConnectionStatus.NOT_CONNECTED, "Connection closed", ex);
        }
    }
}