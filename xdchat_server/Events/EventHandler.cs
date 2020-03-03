using System;

namespace xdchat_server.Events {
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        public string Filter { get; set; }

        public EventPriority Priority { get; } = EventPriority.Default;
    }
}