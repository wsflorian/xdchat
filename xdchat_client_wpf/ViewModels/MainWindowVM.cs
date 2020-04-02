using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using xdchat_client_wpf.Annotations;
using XdChatShared;


namespace xdchat_client_wpf
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Page _connectionPage;
        private Page _chatPage;
        private bool _chatEnabled;

        public Page ConnectionPage
        {
            get => _connectionPage;
            set
            {
                value.DataContext = new ConnectionPageVM(ClientConnectedFunc);
                _connectionPage = value;
                PropChanged(nameof(ConnectionPage));
            }
        }

        public Page ChatPage
        {
            get => _chatPage;
            set
            {
                _chatPage = value;
                PropChanged(nameof(ChatPage));
            }
        }

        public MainWindowVM()
        {
            ConnectionPage = new ConnectionPage();
            ChatPage = new ChatPage();
            ChatEnabled = false;
        }

        public bool ChatEnabled
        {
            get => _chatEnabled;
            set
            {
                _chatEnabled = value; 
                PropChanged(nameof(ChatEnabled));
            }
        }

        private void ClientConnectedFunc()
        {
            var data = (ConnectionPageVM) ConnectionPage.DataContext;
            ChatEnabled = true;
        }
        
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}