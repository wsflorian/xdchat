using System.Net.Sockets;
using Microsoft.Win32;
using xdchat_client;
using XdChatShared.Events;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf {
    public class XdClient {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\XdClient";
        private const string RegistryUuidValueName = "uuid";
        private const string RegistryNicknameValueName = "nickname";
        
        public static XdClient Instance { get; } = new XdClient();
        
        public EventEmitter Emitter { get; } = new EventEmitter();
        public XdServerConnection Connection { get; set; }

        public string Nickname {
            get => (string)Registry.GetValue(RegistryPath, RegistryNicknameValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryNicknameValueName, value);
        }

        public string Uuid {
            get => (string)Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryUuidValueName, value);
        }

        public void Connect(string host, ushort port) {
            try {
                TcpClient client = new TcpClient(host, port);
                
                this.Connection = new XdServerConnection(client, Nickname, Uuid);
                
            } catch (SocketException e) {
                
            }
        }
    }
}