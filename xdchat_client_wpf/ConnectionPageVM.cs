using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using xdchat_client_wpf.Annotations;
using XdChatShared;


namespace xdchat_client_wpf
{
    public class ConnectionPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _serverAdress;
        private string _nickname;
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
        
        public ActionCommand ConnectButtonActionCommand { get; private set; }
        private Action ClientConnectedFunc { get; set; }

        public ConnectionPageVM(Action clientConnectedFunc)
        {
            ConnectButtonActionCommand = new ActionCommand(ClickConnectFunc, ConnectButtonClickable);
            ClientConnectedFunc = clientConnectedFunc;
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