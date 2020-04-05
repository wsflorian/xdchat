using System.ComponentModel;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace xdchat_client_wpf.ViewModels {
    public class MainWindowVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private Page _connectionPage, _chatPage;
        private bool _chatEnabled;
        private int _selectedIndex;
        private string _windowTitle;
        
        [UsedImplicitly] public string WindowTitle { 
            get => (string.IsNullOrEmpty(_windowTitle) ? "" : _windowTitle + " - ") + "xdchat"  ;
            set {
                _windowTitle = value;
                PropChanged(nameof(WindowTitle));
            }
        }
        
        [UsedImplicitly] public Page ConnectionPage {
            get => _connectionPage;
            set {
                _connectionPage = value;
                PropChanged(nameof(ConnectionPage));
            }
        }

        [UsedImplicitly] public Page ChatPage {
            get => _chatPage;
            set {
                _chatPage = value;
                PropChanged(nameof(ChatPage));
            }
        }
        
        [UsedImplicitly] public bool ChatEnabled {
            get => _chatEnabled;
            set {
                _chatEnabled = value;
                PropChanged(nameof(ChatEnabled));
            }
        }

        [UsedImplicitly] public int SelectedIndex {
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
            ChatPage.DataContext = new ChatPageVM(ChatPage);
            
            WindowTitle = null;
            ChatEnabled = false;
        }

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}