using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleLogger;
using XdChatShared.Scheduler;

namespace xdchat_server.Events {
    public class EventListener {
        private readonly IDictionary<Type, List<EventRegistration>> registrations = new Dictionary<Type, List<EventRegistration>>();

        public void Register(Action<EventListener> listenerConsumer) {
            GetType().GetMethods().ToList().ForEach(info => {
                object[] attributes = info.GetCustomAttributes(typeof(EventHandler), false);
                
                if (attributes.Length < 1) return;
                if (info.GetParameters().Length < 1) {
                    Logger.Log(Logger.Level.Warning, "EventHandler methods require at least one parameter");
                    return;
                }

                Type parameterType = info.GetParameters()[0].ParameterType;
                if (!parameterType.IsSubclassOf(typeof(Event))) {
                    Logger.Log(Logger.Level.Warning, "EventHandler parameter must be of type Event");
                    return;
                }
                
                GetHandlerMethods(parameterType).Add(new EventRegistration((EventHandler) attributes[0], info));
            });
            
            listenerConsumer.Invoke(this);
        }

        public void Emit<T>(T ev) where T: Event {
            XdScheduler.Instance.CheckIsSync();
            
            GetHandlerMethods(ev.GetType()).ForEach(info => {
                if (ev.GetType().IsSubclassOf(typeof(IEventFilter)) && info.HandlerInfo.Filter != null) {
                    IEventFilter filter = (IEventFilter) ev;

                    if (!filter.DoesMatch(info.HandlerInfo.Filter)) {
                        return;
                    }
                }

                info.Method.Invoke(this, new object[] {ev});
            });
        }
        
        private List<EventRegistration> GetHandlerMethods(Type type) {
            if (!registrations.TryGetValue(type, out List<EventRegistration> listeners)) {
                registrations[type] = listeners = new List<EventRegistration>();
            }

            return listeners;
        }
    }
    
    public class EventRegistration {
        public EventHandler HandlerInfo { get; }
        public MethodInfo Method { get; }

        public EventRegistration(EventHandler handlerInfo, MethodInfo method) {
            HandlerInfo = handlerInfo;
            Method = method;
        }
    }
}