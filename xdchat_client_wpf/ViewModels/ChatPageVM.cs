using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using xdchat_client_wpf.Annotations;
using xdchat_client_wpf.EventsImpl;
using xdchat_client_wpf.Models;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Events;
using XdChatShared.Misc;
using XdChatShared.Packets;
using XdChatShared.Scheduler;

namespace xdchat_client_wpf.ViewModels {
    public class ChatPageVM : INotifyPropertyChanged, IEventListener {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<ChatMessage> _chatLog;
        private ObservableCollection<ServerPacketClientList.User> _userList;
        private string _message;
        private bool _inputEnabled;
        private readonly TextBox _messageBox;
        private readonly ListBox _messageList;

        [UsedImplicitly] public ObservableCollection<ChatMessage> ChatLog {
            get => _chatLog;
            set {
                _chatLog = value;
                PropChanged(nameof(ChatLog));
            }
        }

        [UsedImplicitly] public ObservableCollection<ServerPacketClientList.User> UserList {
            get => _userList;
            set {
                _userList = value;
                PropChanged(nameof(UserList));
            }
        }

        [UsedImplicitly] public string Message {
            get => _message;
            set {
                _message = value;
                PropChanged(nameof(Message));
            }
        }

        [UsedImplicitly] public bool InputEnabled {
            get => _inputEnabled;
            set {
                _inputEnabled = value;
                PropChanged(nameof(InputEnabled));
            }
        }

        [UsedImplicitly] public ActionCommand SendMessageCommand { get; }
        
        public ChatPageVM(Page chatPage) {
            this._messageBox = (TextBox) chatPage.FindName("MessageBox");
            this._messageList = (ListBox) chatPage.FindName("MessageList");
            
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

                XdScheduler.QueueAsyncTaskScheduled(() => {
                    Application.Current?.Dispatcher?.BeginInvoke(new Func<bool>(_messageBox.Focus));
                }, 50);
            });
        }

        private bool CanSendMessage() {
            return Message.Trim().Length > 0 && InputEnabled;
        }

        private void AddChatMessage(string user, string message, DateTime? time = null, string relevantText = null) {
            Application.Current?.Dispatcher?.Invoke(() => {
                if (VisualTreeHelper.GetChildrenCount(this._messageList) <= 0) return;
                Border border = (Border) VisualTreeHelper.GetChild(this._messageList, 0);
                ScrollViewer scrollViewer = (ScrollViewer) VisualTreeHelper.GetChild(border, 0);
                bool wasAtBottom = scrollViewer.VerticalOffset.Equals(scrollViewer.ScrollableHeight);

                ChatMessage chatMessage = new ChatMessage(time ?? DateTime.Now, message, user);
                
                AddCopyHandler(chatMessage.ContextMenu, "Copy text", message);
                if (relevantText != null) {
                    AddCopyHandler(chatMessage.ContextMenu, "Copy relevant text", relevantText);
                }
                
                ChatLog.Add(chatMessage);

                if (wasAtBottom) {
                    scrollViewer.ScrollToBottom();
                }
            });
        }

        private static void AddCopyHandler(ContextMenu menu, string title, string textToCopy) {
            MenuItem menuItem = new MenuItem {
                Header = title
            };
            menuItem.Click += (sender, args) => Clipboard.SetText(textToCopy);
            menu.Items.Add(menuItem);
        }

        private string GetUserNameByUuid(string uuid) {
            if (uuid == null) return "[Server]";
            return UserList.FirstOrDefault(e => e.HashedUuid == uuid)?.Nickname ?? "[Unknown]";
        }

        [XdEventHandler(typeof(ServerPacketClientList))]
        public void HandleUserListUpdate(PacketReceivedEvent evt) {
            ServerPacketClientList packet = (ServerPacketClientList) evt.Packet;
            List<ServerPacketClientList.User> users = new List<ServerPacketClientList.User>(packet.Users) {
                new ServerPacketClientList.User(XdClient.Instance.Nickname,
                    Helper.Sha256Hash(XdClient.Instance.Uuid))
            };

            UserList = new ObservableCollection<ServerPacketClientList.User>(users.OrderBy(user => user.Nickname));
        }

        [XdEventHandler(typeof(ServerPacketChatMessage))]
        public void HandleIncomingChatMessage(PacketReceivedEvent evt) {
            ServerPacketChatMessage packet = (ServerPacketChatMessage) evt.Packet;

            AddChatMessage(GetUserNameByUuid(packet.HashedUuid), packet.Text, null, packet.CopyText);
        }

        [XdEventHandler(typeof(ServerPacketOldChatMessage))]
        public void HandleOldChatMessage(PacketReceivedEvent ev) {
            ServerPacketOldChatMessage packet = (ServerPacketOldChatMessage) ev.Packet;
            AddChatMessage(GetUserNameByUuid(packet.HashedUuid), packet.Text, packet.Timestamp);
        }

        [XdEventHandler(typeof(ServerPacketChatClear))]
        public void HandleChatClear(PacketReceivedEvent ev) {
            Application.Current.Dispatcher.Invoke(() => ChatLog.Clear());
        }

        protected virtual void PropChanged(string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}