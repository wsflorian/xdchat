using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using JetBrains.Annotations;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Connection;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf.ViewModels {
    public class ConnectionPageVM : INotifyPropertyChanged, IEventListener {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAddress;
        private string _nickname;
        private string _buttonText;
        private ActionCommand _connectButtonActionCommand;

        [UsedImplicitly] public string ServerAddress {
            get => _serverAddress;
            set {
                _serverAddress = value;
                PropChanged(nameof(_serverAddress));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        [UsedImplicitly] public string Nickname {
            get => _nickname;
            set {
                if (value == null || value.Length > Helper.MaxNickLength) return;
                _nickname = value;
                PropChanged(nameof(Nickname));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        [UsedImplicitly] public string ButtonText {
            get => _buttonText;
            set {
                _buttonText = value;
                PropChanged(nameof(ButtonText));
            }
        }

        [UsedImplicitly] public bool TextFieldsEnabled {
            get => !Connecting && XdClient.Instance.Status != XdConnectionStatus.Connected;
        }

        [UsedImplicitly] public ObservableCollection<ServerLogMessage> ServerLog {
            get => XdClient.Instance.LogMessages;
        }
        
        private bool Connecting => XdClient.Instance.Status == XdConnectionStatus.Connecting || XdClient.Instance.Status == XdConnectionStatus.Authenticating;

        [UsedImplicitly] public ActionCommand ConnectButtonActionCommand {
            get => _connectButtonActionCommand;
            set {
                _connectButtonActionCommand = value;
                PropChanged(nameof(ConnectButtonActionCommand));
            }
        }
        
        private MainWindowVM MainWindow { get; }

        public ConnectionPageVM(MainWindowVM mainWindow) {
            this.MainWindow = mainWindow;

            ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
            ButtonText = "Connect to Server";

            Nickname = XdClient.Instance.Nickname;
            ServerAddress = XdClient.Instance.HostName != null
                ? $"{XdClient.Instance.HostName}:{XdClient.Instance.PortName}"
                : "";
            
            XdClient.Instance.EventEmitter.RegisterListener(this);
        }

        private void ClickConnectFunc() {
            XdClient.Instance.Nickname = Nickname;
            if (XdClient.Instance.Uuid == null) {
                XdClient.Instance.Uuid = Guid.NewGuid().ToString();
            }

            if (XdConnection.TryParseEndpoint(ServerAddress, Helper.DefaultPort, out string host, out ushort port)) {
                XdClient.Instance.HostName = host;
                XdClient.Instance.PortName = port;
            }

            XdScheduler.QueueAsyncTask(XdClient.Instance.Connect);

            AddLogMessage("Connecting...");
        }

        private void ClickDisconnectFunc() {
            XdScheduler.QueueSyncTask(XdClient.Instance.Disconnect);
        }

        public void AddLogMessage(string message) {
            Action<ServerLogMessage> addMethod = ServerLog.Add;
            Application.Current?.Dispatcher?.BeginInvoke(addMethod, new ServerLogMessage(){Message = message, TimeStamp = DateTime.Now});
        }
        
        private bool ConnectButtonClickable() {
            return Validation.IsValidNickname(Nickname) &&
                   Validation.IsValidHostPort(ServerAddress) &&
                   !Connecting;
        }

        [XdEventHandler(typeof(ServerPacketDisconnect))]
        public void OnDisconnectInfo(PacketReceivedEvent evt) {
            ServerPacketDisconnect packet = (ServerPacketDisconnect) evt.Packet;
            AddLogMessage("Disconnected: " + packet.Text);
        }
        
        [XdEventHandler]
        public void HandleStatusUpdate(ConnectionStatusEvent evt) {
            string message = evt.Info;
            if (evt.Error != null && !IsSocketReadInterrupt(evt.Error) && !(evt.Error is EndOfStreamException)) {
                message += "\nException: " + evt.Error.Message;
            }

            AddLogMessage(message);
            PropChanged(nameof(TextFieldsEnabled));
            ConnectButtonActionCommand.RaiseCanExecuteChanged();

            switch (evt.Status) {
                case XdConnectionStatus.NotConnected:
                    ButtonText = "Connect to Server";
                    MainWindow.WindowTitle = null;
                    
                    ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
                    MainWindow.ChatEnabled = false;
                    MainWindow.SelectedIndex = 0;
                    
                    XdClient.Instance.Disconnect();
                    break;
                case XdConnectionStatus.Connecting:
                case XdConnectionStatus.Authenticating:
                    ButtonText = "Waiting to connect...";
                    break;
                case XdConnectionStatus.Connected:
                    ButtonText = "Disconnect";
                    MainWindow.WindowTitle = XdClient.Instance.HostName + ":" + XdClient.Instance.PortName;
                    
                    ConnectButtonActionCommand = new ActionCommand(ClickDisconnectFunc, ConnectButtonClickable);
                    MainWindow.ChatEnabled = true;
                    MainWindow.SelectedIndex = 1;
                    
                    ChatPageVM chatPageVm = (ChatPageVM)this.MainWindow?.ChatPage.DataContext;
                    chatPageVm.ChatLog.Clear();
                    chatPageVm.UserList.Clear();
                    
                    break;
            }
        }
        
        private static bool IsSocketReadInterrupt(Exception ex) {
            return ex.InnerException != null 
                   && ex.InnerException is SocketException socketEx
                   && socketEx.SocketErrorCode == SocketError.Interrupted;
        } 

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}