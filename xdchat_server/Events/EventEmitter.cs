using System;
using System.Collections.Generic;
using XdChatShared.Scheduler;

namespace xdchat_server.Events {
    public class EventEmitter {
        private readonly List<EventListener> listeners = new List<EventListener>();

        public void RegisterListener(EventListener listener) {
            listener.Register(listeners.Add);
        }
        
        public T Emit<T>(T ev) where T : Event {
            XdScheduler.Instance.CheckIsSync();
            listeners.ForEach(listener => listener.Emit(ev));
            return ev;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        public string Filter { get; set; }
    }
}