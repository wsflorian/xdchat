using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using xdchat_client_wpf.Models;
using XdChatShared;
using XdChatShared.Scheduler;


namespace xdchat_client_wpf {
    public class ConnectionPageVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAddress;
        private string _nickname;

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

        public ObservableCollection<ServerLogMessage> ServerLog {
            get => XdClient.Instance.LogMessages;
        }

        public ActionCommand ConnectButtonActionCommand { get; private set; }
        private MainWindowVM MainWindow { get; }

        public ConnectionPageVM(MainWindowVM mainWindow) {
            ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
            this.MainWindow = mainWindow;

            Nickname = XdClient.Instance.Nickname;
            ServerAdress = XdClient.Instance.HostName != null
                ? $"{XdClient.Instance.HostName}:{XdClient.Instance.PortName}"
                : "";
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
            // should be executed after client receives auth pack of server
            MainWindow.ClientConnectedFunc();
        }

        public static void AddLogMessage(ICollection<ServerLogMessage> collection, ServerLogMessage item) {
            Action<ServerLogMessage> addMethod = XdClient.Instance.LogMessages.Add;
            Application.Current?.Dispatcher?.BeginInvoke(addMethod, item);
        }

        private bool ConnectButtonClickable() {
            return XdChatShared.Validation.IsValidNickname(Nickname) &&
                   XdChatShared.Validation.IsValidHostPort(ServerAdress);
        }

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}