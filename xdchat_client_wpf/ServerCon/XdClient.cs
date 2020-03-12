using System;
using System.Net.Sockets;
using Microsoft.Win32;
using xdchat_client;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.ServerCon;
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

        public string UuidShort => Uuid.Substring(0, 8);

        public void Connect(string host, ushort port) {
            if (Connection != null)
                throw new InvalidOperationException("Already connected");
            
            try {
                UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.CONNECTING, $"Connecting to {host}:{port}"));
                TcpClient client = new TcpClient(host, port);
                
                UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.AUTHENTICATING, $"Authenticating as {Nickname} ({UuidShort})"));
                this.Connection = new XdServerConnection(client, Nickname, Uuid);
                UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.CONNECTED, "Connection established"));
            } catch (SocketException e) {
                UpdateStatus(new ConnectionStatusEvent(XdConnectionStatus.NOT_CONNECTED, "Connection closed", e));
            }
        }

        private void UpdateStatus(ConnectionStatusEvent ev) {
            this.Status = ev.Status;
            Emitter.EmitSync(ev);
        }
    }
}