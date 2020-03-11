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
    class XdServerConnection : XdConnection {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\XdClient";
        private const string RegistryUuidValueName = "uuid";
        private const string RegistryNicknameValueName = "nickname";

        private TcpClient Client { get; }
        private string Uuid { get; set; }
        private string Nickname { get; set; }

        public void OldCode() {
             Nickname = (string)Registry.GetValue(RegistryPath, RegistryNicknameValueName, null);
             Uuid = (string)Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
             XdScheduler.Instance.RunSync(() => SendMessage(""));
             
             Registry.SetValue(RegistryPath, RegistryUuidValueName, Uuid);
             Registry.SetValue(RegistryPath, RegistryNicknameValueName, Nickname);
        }
        
        public XdServerConnection(TcpClient client, string nickname, Guid uuid) {
            XdScheduler.Instance.CheckIsSync();
            
            this.Client = client;
            this.Nickname = nickname;
            this.Uuid = uuid.ToString();
            
            base.Initialize(client);
            SendAuth();
        }

        private void SendAuth() {
            base.Send(new ClientPacketAuth {
                Nickname = Nickname, Uuid = Uuid
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