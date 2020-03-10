using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLogger;
using XdChatShared.Scheduler;

namespace XdChatShared.Events {
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
                
                GetEventRegistrations(parameterType).Add(new EventRegistration((EventHandler) attributes[0], info, this));
            });
            
            listenerConsumer.Invoke(this);
        }

        public List<EventRegistration> GetEventRegistrations(Type type) {
            if (!registrations.TryGetValue(type, out List<EventRegistration> listeners)) {
                registrations[type] = listeners = new List<EventRegistration>();
            }

            return listeners;
        }
    }
}