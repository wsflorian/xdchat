using System.Collections.Generic;
using System.Linq;
using XdChatShared.Scheduler;

namespace XdChatShared.Events {
    public class EventEmitter {
        private readonly List<EventListener> listeners = new List<EventListener>();

        public void RegisterListener(EventListener listener) {
            listener.Register(listeners.Add);
        }
        
        public T Emit<T>(T ev) where T : Event {
            XdScheduler.CheckIsMainThread();
            
            listeners
                .SelectMany(registration => registration.GetEventRegistrations(ev.GetType()))
                .Where(registration => {
                    if (!(ev is IEventFilter) || registration.HandlerInfo.Filter == null)
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
}