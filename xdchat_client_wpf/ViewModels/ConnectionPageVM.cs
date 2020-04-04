using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using xdchat_client;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_client_wpf.ServerCon;
using XdChatShared;
using XdChatShared.Events;
using XdChatShared.Scheduler;


namespace xdchat_client_wpf {
    public class ConnectionPageVM : INotifyPropertyChanged, IEventListener {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAddress;
        private string _nickname;
        private string _buttonText;
        private bool _textFieldsEnabled;
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
        
        private bool Connecting => XdClient.Instance.Status == XdConnectionStatus.CONNECTING || XdClient.Instance.Status == XdConnectionStatus.AUTHENTICATING;


        public ActionCommand ConnectButtonActionCommand {
            get => _connectButtonActionCommand;
            set {
                _connectButtonActionCommand = value;
                PropChanged(nameof(ConnectButtonActionCommand));
            }
        }
        
        private MainWindowVM MainWindow { get; }

        public ConnectionPageVM(MainWindowVM mainWindow) {
            // this.TextFieldsEnabled = true;
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

            if (XdConnection.TryParseEndpoint(ServerAdress, Constants.DefaultPort, out string host, out ushort port)) {
                XdClient.Instance.HostName = host;
                XdClient.Instance.PortName = port;
            }

            XdScheduler.QueueAsyncTask(XdClient.Instance.Connect);

            ServerLog.Add(new ServerLogMessage() {Message = "Connecting...", TimeStamp = DateTime.Now});
            MainWindow.ChatEnabled = true;
        }

        private void ClickDisconnectFunc() {
            ServerLog.Add(new ServerLogMessage() {Message = "Disconnected...", TimeStamp = DateTime.Now});
            MainWindow.ChatEnabled = false;
        }

        public static void AddLogMessage(ICollection<ServerLogMessage> collection, ServerLogMessage item) {
            Action<ServerLogMessage> addMethod = XdClient.Instance.LogMessages.Add;
            Application.Current?.Dispatcher?.BeginInvoke(addMethod, item);
        }

        private bool ConnectButtonClickable() {
            return XdChatShared.Validation.IsValidNickname(Nickname) &&
                   XdChatShared.Validation.IsValidHostPort(ServerAdress) &&
                   !Connecting;
        }
        
        [XdChatShared.Events.EventHandler]
        public void HandleStatusUpdate(ConnectionStatusEvent evt) {
            PropChanged(nameof(TextFieldsEnabled));
            PropChanged(nameof(Connecting));
            
            switch (evt.Status) {
                case XdConnectionStatus.NOT_CONNECTED:
                    ButtonText = "Connect to Server";
                    break;
                case XdConnectionStatus.CONNECTING:
                    ButtonText = "Waiting to connect...";
                    break;
                case XdConnectionStatus.AUTHENTICATING:
                    ButtonText = "Waiting to connect...";
                    break;
                case XdConnectionStatus.CONNECTED:
                    ButtonText = "Disconnect";
                    break;
            }
        }

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}