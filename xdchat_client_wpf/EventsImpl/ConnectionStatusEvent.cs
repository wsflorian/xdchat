using System;
using xdchat_client_wpf.ServerCon;
using XdChatShared.Events;

namespace xdchat_client_wpf.EventsImpl {
    public class ConnectionStatusEvent : Event {
        public XdConnectionStatus Status { get; }
        public string Info { get; }
        public Exception Error { get; }

        public ConnectionStatusEvent(XdConnectionStatus status, string info, Exception error = null) {
            Status = status;
            Info = info;
            Error = error;
        }
    }
}