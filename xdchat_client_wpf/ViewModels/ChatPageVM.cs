using System.ComponentModel;

namespace xdchat_client_wpf
{
    public class ChatPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ChatPageVM()
        {
            
        }
        
        
        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}