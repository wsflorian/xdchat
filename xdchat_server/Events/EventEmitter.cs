using System;
using System.Collections.Generic;
using System.Linq;
using XdChatShared.Scheduler;

namespace xdchat_server.Events {
    public class EventEmitter {
        private readonly List<EventListener> listeners = new List<EventListener>();

        public void RegisterListener(EventListener listener) {
            listener.Register(listeners.Add);
        }
        
        public T Emit<T>(T ev) where T : Event {
            XdScheduler.Instance.CheckIsSync();
            
            listeners
                .SelectMany(registration => registration.GetEventRegistrations(ev.GetType()))
                .Where(registration => {
                    if (!ev.GetType().IsSubclassOf(typeof(IEventFilter)) || registration.HandlerInfo.Filter == null)
                        return true; // Don't apply filter if not filterable or no filter is set
                    
                    IEventFilter filter = (IEventFilter) ev;
                    return filter.DoesMatch(registration.HandlerInfo.Filter);
                })
                .OrderByDescending(registration => registration.HandlerInfo.Priority)
                .ToList()
                .ForEach(registration => {
                    registration.Method.Invoke(registration.Listener, new object[] {ev});
                });
            
            return ev;
        }
    }
    
    public enum EventPriority {
        Internal, // Used for internal connection things (e.g. Auth packets)
        Default
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute {
        public string Filter { get; set; }

        public EventPriority Priority { get; } = EventPriority.Default;
    }
}