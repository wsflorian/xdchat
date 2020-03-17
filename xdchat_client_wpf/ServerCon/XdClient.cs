using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using xdchat_client;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Events;
using XdChatShared;
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
        public XdServerConnection Connection { get; private set; }
        public XdConnectionStatus Status { get; private set; } 
        
        public string Nickname {
            get => (string)Registry.GetValue(RegistryPath, RegistryNicknameValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryNicknameValueName, value);
        }

        public string Uuid {
            get => (string)Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryUuidValueName, value);
        }
        
        public string HostName {
            get => (string)Registry.GetValue(RegistryPath, RegistryHostValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryHostValueName, value);
        }

        public ushort PortName {
            get {
                int value = (int) Registry.GetValue(RegistryPath, RegistryPortValueName, (int) Constants.DefaultPort);
                return (ushort) value;
            }
            set => Registry.SetValue(RegistryPath, RegistryPortValueName, (int) value);
        }

        public string UuidShort => Uuid.Substring(0, 8);

        public async Task Connect() {
            if (Connection != null)
                throw new InvalidOperationException("Already connected");
            
            try {
                await UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.CONNECTING, $"Connecting to {HostName}:{PortName}"));
                TcpClient client = new TcpClient(HostName, PortName);
                
                await UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.AUTHENTICATING, $"Authenticating as {Nickname} ({UuidShort})"));
                this.Connection = new XdServerConnection(client, Nickname, Uuid);
                await UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.CONNECTED, "Connection established"));
            } catch (SocketException e) {
                await UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.NOT_CONNECTED, "Connection failed", e));
            }
        }

        public Task UpdateStatus(ConnectionStatusEvent ev) {
            return XdScheduler.QueueSyncTask(() => {
                this.Status = ev.Status;
                Emitter.Emit(ev);
            });
        }
    }
}