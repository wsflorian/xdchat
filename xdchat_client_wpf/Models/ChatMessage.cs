using System;
using xdchat_client_wpf.Annotations;

namespace xdchat_client_wpf.Models {
    public class ChatMessage {
        [UsedImplicitly] public DateTime TimeStamp { get; }
        [UsedImplicitly] public string Message { get; }
        [UsedImplicitly] public string User { get; }

        public ChatMessage(DateTime timeStamp, string message, string user) {
            TimeStamp = timeStamp;
            Message = message;
            User = user;
        }
    }
}