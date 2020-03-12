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
        private const string RegistryHostValueName = "hostName";
        private const string RegistryPortValueName = "portName";

        private XdClient(){}
        
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

        public string HostName
        {
            get => (string)Registry.GetValue(RegistryPath, RegistryHostValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryHostValueName, value);
        }

        public ushort PortName
        {
            get
            {
                if (ushort.TryParse((string) Registry.GetValue(RegistryPath, RegistryPortValueName, 0), out ushort port))
                {
                    return port;
                }

                return 10000;
            }
            set => Registry.SetValue(RegistryPath, RegistryPortValueName, value);
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