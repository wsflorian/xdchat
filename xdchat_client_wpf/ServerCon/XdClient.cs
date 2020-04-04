using System;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Win32;
using SimpleLogger;
using SimpleLogger.Logging.Handlers;
using xdchat_client;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_client_wpf.ServerCon;
using XdChatShared;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf {
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
            get => (string) Registry.GetValue(RegistryPath, RegistryNicknameValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryNicknameValueName, value);
        }

        public string Uuid {
            get => (string) Registry.GetValue(RegistryPath, RegistryUuidValueName, null);
            set => Registry.SetValue(RegistryPath, RegistryUuidValueName, value);
        }

        public string HostName {
            get => (string) Registry.GetValue(RegistryPath, RegistryHostValueName, null);
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
            XdScheduler.CheckIsNotMainThread();

            if (Connection != null)
                throw new InvalidOperationException("Already connected");

            try {
                // Event must be sent sync
                await XdScheduler.QueueSyncTask(() =>
                    UpdateStatus(XdConnectionStatus.CONNECTING, $"Connecting to {HostName}:{PortName}"));

                // Connection is done async because it can block up to 30 seconds (timeout)
                TcpClient client = new TcpClient(HostName, PortName);

                // Future actions are done sync again
                await XdScheduler.QueueSyncTask(() => {
                    UpdateStatus(XdConnectionStatus.AUTHENTICATING, $"Authenticating as {Nickname} ({UuidShort})");
                    this.Connection = new XdServerConnection();
                    this.Connection.Initialize(client);
                    UpdateStatus(XdConnectionStatus.CONNECTED, "Connection established");
                });
            }
            catch (SocketException e) {
                await XdScheduler.QueueSyncTask(() =>
                    UpdateStatus(XdConnectionStatus.NOT_CONNECTED, "Connection failed", e));
            }
        }

        public void Disconnect() {
            XdScheduler.CheckIsMainThread();

            this.Connection.End();
            this.Connection = null;
        }

        public void UpdateStatus(XdConnectionStatus status, string message, Exception e = null) {
            this.Status = status;
            EventEmitter.Emit(new ConnectionStatusEvent(status, message, e));
        }

        public override void Start() {
            string logPath = Environment.GetEnvironmentVariable("LOG_PATH");
            if (logPath != null) {
                Logger.LoggerHandlerManager.AddHandler(new FileLoggerHandler(new XdLoggerFormatter(), logPath));
            }

            Logger.LoggerHandlerManager.AddHandler(new TraceLoggerHandler());
        }

        public override void Stop() {
        }
    }
}