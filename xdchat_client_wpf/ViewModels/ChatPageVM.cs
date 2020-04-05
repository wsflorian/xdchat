using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Packets;

namespace xdchat_client_wpf
{
    public class ChatPageVM : INotifyPropertyChanged, IEventListener
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ChatMessage> _chatLog;
        private ObservableCollection<ServerPacketClientList.User> _userList;

        public ObservableCollection<ChatMessage> ChatLog {
            get => _chatLog;
            set {
                _chatLog = value;
                PropChanged(nameof(ChatLog));
            }
        }

        public ObservableCollection<ServerPacketClientList.User> UserList {
            get => _userList;
            set {
                _userList = value;
                PropChanged(nameof(UserList));
            }
        }
        
        public MainWindowVM MainWindow { get; set; }
        
        public ChatPageVM(MainWindowVM mainWindow)
        {
            this.MainWindow = mainWindow;
            ChatLog = new ObservableCollection<ChatMessage>();
            XdClient.Instance.EventEmitter.RegisterListener(this);
        }
        
        public void AddChatMessage(string user, string message) {
            Action<ChatMessage> addMethod = ChatLog.Add;
            Application.Current?.Dispatcher?.BeginInvoke(addMethod, 
                new ChatMessage(){ User = user, Message = message, TimeStamp = DateTime.Now });
        }

        protected virtual void PropChanged( string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [XdChatShared.Events.EventHandler(Filter = typeof(ServerPacketClientList))]
        public void HandleUserListUpdate(PacketReceivedEvent evt) {
            var packet = (ServerPacketClientList) evt.Packet;
            packet.Users.Add(new ServerPacketClientList.User(
                XdClient.Instance.Nickname, 
                Helper.Sha256Hash(XdClient.Instance.Uuid)));
            
            Trace.WriteLine($"{XdClient.Instance.Nickname}: {String.Join("; ", packet.Users.Select(e => e.Nickname))}");
            
            UserList = new ObservableCollection<ServerPacketClientList.User>(packet.Users);
        }
        
        [XdChatShared.Events.EventHandler(Filter = typeof(ServerPacketClientList))]
        public void HandleIncomingChatMessage(PacketReceivedEvent evt) {
            var packet = (ServerPacketChatMessage) evt.Packet;
        }
    }
}