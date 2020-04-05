using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf
{
    public class ChatPageVM : INotifyPropertyChanged, IEventListener
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ChatMessage> _chatLog;
        private ObservableCollection<ServerPacketClientList.User> _userList;
        private string _message;
        private bool _inputEnabled;

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

        public string Message {
            get => _message;
            set {
                _message = value;
                PropChanged(nameof(Message));
            }
        }

        public bool InputEnabled {
            get => _inputEnabled;
            set {
                _inputEnabled = value;
                PropChanged(nameof(InputEnabled));
            }
        }
        
        public ActionCommand SendMessageCommand { get; set; }
        
        public MainWindowVM MainWindow { get; set; }
        
        public ChatPageVM(MainWindowVM mainWindow)
        {
            this.MainWindow = mainWindow;
            ChatLog = new ObservableCollection<ChatMessage>();
            Message = "";
            InputEnabled = true;
            SendMessageCommand = new ActionCommand(SendMessage, CanSendMessage);
            
            XdClient.Instance.EventEmitter.RegisterListener(this);
        }
        
        private void SendMessage() {
            InputEnabled = false;
            XdScheduler.QueueSyncTask(() => {
                XdClient.Instance.Connection.SendMessage(Message.Trim());
                AddChatMessage(XdClient.Instance.Nickname, Message);
                Message = "";
                InputEnabled = true;
            });
        }

        private bool CanSendMessage() {
            return Message.Trim().Length > 0 && InputEnabled;
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

        private string GetUserNameByUuid(string uuid) {
            if (uuid == null) return "Server";
            return UserList.FirstOrDefault(e => e.HashedUuid == uuid)?.Nickname ?? "Unknown";
        }

        [XdChatShared.Events.EventHandler(Filter = typeof(ServerPacketClientList))]
        public void HandleUserListUpdate(PacketReceivedEvent evt) {
            var packet = (ServerPacketClientList) evt.Packet;
            packet.Users.Add(new ServerPacketClientList.User(
                XdClient.Instance.Nickname, 
                Helper.Sha256Hash(XdClient.Instance.Uuid)));
            
            UserList = new ObservableCollection<ServerPacketClientList.User>(packet.Users.OrderBy(e => e.Nickname));
        }
        
        [XdChatShared.Events.EventHandler(Filter = typeof(ServerPacketChatMessage))]
        public void HandleIncomingChatMessage(PacketReceivedEvent evt) {
            var packet = (ServerPacketChatMessage) evt.Packet;
            
            AddChatMessage(GetUserNameByUuid(packet.HashedUuid), packet.Text);
        }
    }
}