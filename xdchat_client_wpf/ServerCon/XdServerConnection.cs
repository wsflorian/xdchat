using Microsoft.Win32;
using System.Linq;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using XdChatShared;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client {
    public class XdServerConnection : XdConnection {
        private TcpClient Client { get; }

        public XdServerConnection(TcpClient client, string nickname, string uuid) {
            XdScheduler.Instance.CheckIsSync();
            
            this.Client = client;

            base.Initialize(client);
            SendAuth(nickname, uuid);
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
            Console.WriteLine($"Data received... {Packet.ToJson(packet)}");
        }

        protected override void OnDisconnect(Exception ex) {
            Console.WriteLine($"Disconnected: {ex}");
        }
    }
}