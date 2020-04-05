using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Windows;

using xdchat_client;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_client_wpf.ServerCon;
using XdChatShared;
using XdChatShared.Connection;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Packets;
using XdChatShared.Scheduler;


namespace xdchat_client_wpf {
    public class ConnectionPageVM : INotifyPropertyChanged, IEventListener {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAddress;
        private string _nickname;
        private string _buttonText;
        private ActionCommand _connectButtonActionCommand;

        public string ServerAdress {
            get => _serverAddress;
            set {
                _serverAddress = value;
                PropChanged(nameof(_serverAddress));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public string Nickname {
            get => _nickname;
            set {
                if (value.Length > Helper.MaxNickLength) return;
                _nickname = value;
                PropChanged(nameof(Nickname));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public string ButtonText {
            get => _buttonText;
            set {
                _buttonText = value;
                PropChanged(nameof(ButtonText));
            }
        }

        public bool TextFieldsEnabled {
            get => !Connecting && XdClient.Instance.Status != XdConnectionStatus.CONNECTED;
        }

        public ObservableCollection<ServerLogMessage> ServerLog {
            get => XdClient.Instance.LogMessages;
        }
        
        private bool Connecting { get => XdClient.Instance.Status == XdConnectionStatus.CONNECTING || XdClient.Instance.Status == XdConnectionStatus.AUTHENTICATING; }


        public ActionCommand ConnectButtonActionCommand {
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
            ServerAdress = XdClient.Instance.HostName != null
                ? $"{XdClient.Instance.HostName}:{XdClient.Instance.PortName}"
                : "";
            
            XdClient.Instance.EventEmitter.RegisterListener(this);
        }

        private void ClickConnectFunc() {
            XdClient.Instance.Nickname = Nickname;
            if (XdClient.Instance.Uuid == null) {
                XdClient.Instance.Uuid = Guid.NewGuid().ToString();
            }

            if (XdConnection.TryParseEndpoint(ServerAdress, Helper.DefaultPort, out string host, out ushort port)) {
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
                   Validation.IsValidHostPort(ServerAdress) &&
                   !Connecting;
        }

        [XdChatShared.Events.EventHandler(typeof(ServerPacketDisconnect))]
        public void OnDisconnectInfo(PacketReceivedEvent evt) {
            ServerPacketDisconnect packet = (ServerPacketDisconnect) evt.Packet;
            AddLogMessage("Disconnected: " + packet.Text);
        }
        
        [XdChatShared.Events.EventHandler]
        public void HandleStatusUpdate(ConnectionStatusEvent evt) {
            string message = evt.Info;
            if (evt.Error != null && !IsSocketReadInterrupt(evt.Error) && !(evt.Error is EndOfStreamException)) {
                message += "\nException: " + evt.Error.Message;
            }

            AddLogMessage(message);
            PropChanged(nameof(TextFieldsEnabled));
            ConnectButtonActionCommand.RaiseCanExecuteChanged();

            switch (evt.Status) {
                case XdConnectionStatus.NOT_CONNECTED:
                    ButtonText = "Connect to Server";
                    ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
                    MainWindow.ChatEnabled = false;
                    MainWindow.SelectedIndex = 0;
                    XdClient.Instance.Disconnect();
                    break;
                case XdConnectionStatus.CONNECTING:
                case XdConnectionStatus.AUTHENTICATING:
                    ButtonText = "Waiting to connect...";
                    break;
                case XdConnectionStatus.CONNECTED:
                    ButtonText = "Disconnect";
                    ConnectButtonActionCommand = new ActionCommand(ClickDisconnectFunc, ConnectButtonClickable);
                    MainWindow.ChatEnabled = true;
                    MainWindow.SelectedIndex = 1;
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