using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using xdchat_client_wpf.Annotations;
using xdchat_client_wpf.Models;
using XdChatShared;


namespace xdchat_client_wpf
{
    public class ConnectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAdress;
        private string _nickname;
        private List<ServerLogMessage> _serverLog;
        public string ServerAdress
        {
            get => _serverAdress;
            set
            {
                _serverAdress = value;
                PropChanged(nameof(_serverAdress));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public string Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                PropChanged(nameof(Nickname));
                ConnectButtonActionCommand.RaiseCanExecuteChanged();
            }
        }

        public List<ServerLogMessage> ServerLog
        {
            get => _serverLog;
            set
            {
                _serverLog = value;
                PropChanged(nameof(ServerLog));
            }
        }
        
        public ActionCommand ConnectButtonActionCommand { get; private set; }
        private Action ClientConnectedFunc { get; set; }

        public ConnectionPageVM(Action clientConnectedFunc)
        {
            ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
            ClientConnectedFunc = clientConnectedFunc;
            ServerLog = new List<ServerLogMessage>();
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test1"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test2"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test3"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test4"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaaaaaaabbbbbbbbbbb"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test6"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test7"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test8"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test9"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test10"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test11"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test12"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test13"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test14"});
            ServerLog.Add(new ServerLogMessage(){TimeStamp = new DateTime(), Message = "Test15"});
        }

        private void ClickConnectFunc()
        {
            // should be executed after client receives auth pack of server
            ClientConnectedFunc();
        }

        private bool ConnectButtonClickable()
        {
            return XdChatShared.Validation.IsValidNickname(Nickname) && XdChatShared.Validation.IsValidHostPort(ServerAdress);
        }
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}