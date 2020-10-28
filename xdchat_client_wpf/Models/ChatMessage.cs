using System;
using System.Windows.Controls;
using xdchat_client_wpf.Annotations;

namespace xdchat_client_wpf.Models {
    public class ChatMessage {
        public DateTime TimeStamp { get; }
        [UsedImplicitly] public string TimeStampFormatted => TimeStamp.ToString("dd.MM.YYYY HH:mm:ss");
        [UsedImplicitly] public string Message { get; }
        [UsedImplicitly] public string User { get; }

        [UsedImplicitly] [JetBrains.Annotations.CanBeNull]
        public ContextMenu ContextMenu { set; get; }

        public ChatMessage(DateTime timeStamp, string message, string user) {
            TimeStamp = timeStamp;
            Message = message;
            User = user;
            ContextMenu = new ContextMenu();
        }
    }
}