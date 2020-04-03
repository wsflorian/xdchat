using System.ComponentModel;

namespace xdchat_client_wpf
{
    public class ChatPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowVM MainWindow { get; set; }
        
        public ChatPageVM(MainWindowVM mainWindow)
        {
            this.MainWindow = mainWindow;
        }
        
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}