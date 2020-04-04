using System.ComponentModel;
using System.Windows.Controls;


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
                value.DataContext = new ConnectionPageVM(this);
                _connectionPage = value;
                PropChanged(nameof(ConnectionPage));
            }
        }

        public Page ChatPage
        {
            get => _chatPage;
            set
            {
                value.DataContext = new ChatPageVM(this);
                _chatPage = value;
                PropChanged(nameof(ChatPage));
            }
        }

        public MainWindowVM()
        {
            ConnectionPage = new ConnectionPage();
            ChatPage = new ChatPage();

            ChatEnabled = false;

            var x = XdClient.Instance;
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

        public void ClientConnectedFunc()
        {
            ChatEnabled = true;
        }
        
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}