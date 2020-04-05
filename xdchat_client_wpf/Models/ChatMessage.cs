using System;

namespace xdchat_client_wpf.Models {
    public class ChatMessage {
        public DateTime TimeStamp { get; }
        public string Message { get; }
        public string User { get; }

        public ChatMessage(DateTime timeStamp, string message, string user) {
            TimeStamp = timeStamp;
            Message = message;
            User = user;
        }
    }
}