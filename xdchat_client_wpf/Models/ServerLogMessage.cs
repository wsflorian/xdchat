using System;
using JetBrains.Annotations;

namespace xdchat_client_wpf.Models {
    public class ServerLogMessage {
        [UsedImplicitly] public DateTime TimeStamp { get; set; }
        [UsedImplicitly] public string Message { get; set; }
    }
}