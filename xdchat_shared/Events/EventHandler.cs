using System;

namespace XdChatShared.Events {
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        public object Filter { get; set; }

        public EventPriority Priority { get; } = EventPriority.Default;

        public bool ContextScoped { get; set; }

        public EventHandler() {
        }

        public EventHandler(object filter) {
            Filter = filter;
        }
        
        public EventHandler(object filter, bool contextScoped) {
            Filter = filter;
            ContextScoped = contextScoped;
        }
    }
}