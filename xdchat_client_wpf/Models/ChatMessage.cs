using System;

namespace xdchat_client_wpf.Models {
    public class ChatMessage {
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
    }
}