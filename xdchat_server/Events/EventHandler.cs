using System;

namespace xdchat_server.Events {
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        public object Filter { get; set; }

        public EventPriority Priority { get; } = EventPriority.Default;
    }
}