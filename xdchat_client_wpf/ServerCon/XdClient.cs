using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Win32;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_shared.Logger.Impl;
using XdChatShared;
using XdChatShared.Misc;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf.ServerCon {
    public class XdClient : XdService {
        private const string RegistryPath = @"HKEY_CURRENT_USER\Software\XdClient";
        private const string RegistryUuidValueName = "uuid";
        private const string RegistryNicknameValueName = "nickname";
        private const string RegistryHostValueName = "hostName";
        private const string RegistryPortValueName = "portName";

        private XdClient() {
        }

        public static XdClient Instance { get; } = new XdClient();

        public XdServerConnection Connection { get; private set; }
        public XdConnectionStatus Status { get; private set; }

        public ObservableCollection<ServerLogMessage> LogMessages { get; } =
            new ObservableCollection<ServerLogMessage>();

        public string Nickname {
            get {
                string debugNick = Environment.GetEnvironmentVariable("NICKNAME");
                if (debugNick != null) return debugNick;
                
                return (string) Registry.GetValue(RegistryPath, RegistryNicknameValueName, "");
            }
            set {
                if (Environment.GetEnvironmentVariable("NICKNAME") != null) return;
                Registry.SetValue(RegistryPath, RegistryNicknameValueName, value);
            }
        }

        public string Uuid {
            get {
                string debugUuid = Environment.GetEnvironmentVariable("UUID");
                if (debugUuid != null) return debugUuid;
                
                return (string) Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
            }
            set {
                if (Environment.GetEnvironmentVariable("UUID") != null) return;
                Registry.SetValue(RegistryPath, RegistryUuidValueName, value);
            }
        }

        public string HostName {
            get => (string) Registry.GetValue(RegistryPath, RegistryHostValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryHostValueName, value);
        }

        public ushort PortName {
            get {
                int value = (int) Registry.GetValue(RegistryPath, RegistryPortValueName, (int) Helper.DefaultPort);
                return (ushort) value;
            }
            set => Registry.SetValue(RegistryPath, RegistryPortValueName, (int) value);
        }

        public string UuidShort => Uuid.Substring(0, 8);

        public async Task Connect(bool ssl) {
            XdScheduler.CheckIsNotMainThread();

            if (Connection != null)
                throw new InvalidOperationException("Already connected");

            try {
                // Event must be sent sync
                await XdScheduler.QueueSyncTask(() =>
                    UpdateStatus(XdConnectionStatus.Connecting, $"Connecting to {HostName}:{PortName}"));

                // Connection is done async because it can block up to 30 seconds (timeout)
                TcpClient client = new TcpClient(HostName, PortName);
                Stream stream = ssl ? InitSsl(client) : client.GetStream();

                // Future actions are done sync again
                await XdScheduler.QueueSyncTask(() => {
                    UpdateStatus(XdConnectionStatus.Authenticating, $"Authenticating as {Nickname} ({UuidShort})");
                    this.Connection = new XdServerConnection();
                    this.Connection.Initialize(client, stream);
                    UpdateStatus(XdConnectionStatus.Connected, "Connection established");
                });
            } catch (Exception e) {
                await XdScheduler.QueueSyncTask(() =>
                    UpdateStatus(XdConnectionStatus.NotConnected, "Connection failed", e));
            }
        }

        private Stream InitSsl(TcpClient client) {
            SslStream sslStream = new SslStream(client.GetStream(), false);
            sslStream.AuthenticateAsClient(HostName);
            return sslStream;
        }

        public void Disconnect() {
            XdScheduler.CheckIsMainThread();

            this.Connection?.End();
            this.Connection = null;
        }

        public void UpdateStatus(XdConnectionStatus status, string message, Exception e = null) {
            this.Status = status;
            EventEmitter.Emit(new ConnectionStatusEvent(status, message, e));
        }

        public override void Start() {
            string logPath = Environment.GetEnvironmentVariable("LOG_PATH");
            if (logPath != null) {
                XdLogger.Instance.Publishers.Add(new FileLogPublisher(logPath));
            }

            XdLogger.Instance.Publishers.Add(new TraceLogPublisher());
            
            XdLogger.Info("Client is ready");
        }

        public override void Stop() {
            this.Disconnect();
        }
    }
}