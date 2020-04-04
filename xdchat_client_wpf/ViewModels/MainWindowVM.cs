using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;


namespace xdchat_client_wpf {
    public class MainWindowVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private Page _connectionPage;
        private Page _chatPage;
        private bool _chatEnabled;
        private int _selectedIndex;

        public Page ConnectionPage {
            get => _connectionPage;
            set {
                _connectionPage = value;
                PropChanged(nameof(ConnectionPage));
            }
        }

        public Page ChatPage {
            get => _chatPage;
            set {
                _chatPage = value;
                PropChanged(nameof(ChatPage));
            }
        }
        
        public bool ChatEnabled {
            get => _chatEnabled;
            set {
                _chatEnabled = value;
                PropChanged(nameof(ChatEnabled));
            }
        }

        public int SelectedIndex {
            get => _selectedIndex;
            set {
                _selectedIndex = value;
                PropChanged(nameof(SelectedIndex));
            }
        }

        public MainWindowVM() {
            ConnectionPage = new ConnectionPage();
            ChatPage = new ChatPage();
            
            ConnectionPage.DataContext = new ConnectionPageVM(this);
            ChatPage.DataContext = new ChatPageVM(this);
            
            ChatEnabled = false;
        }

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}