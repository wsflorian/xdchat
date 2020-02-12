using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleLogger;

namespace xdchat_server {
    public class Event {}

    public class CancelableEvent : Event {
        public bool Cancelled { get; set; } = false;
    }

    public class EventEmitter {
        private readonly List<EventListener> listeners = new List<EventListener>();

        public void RegisterListener(EventListener listener) {
            listener.Register(listeners.Add);
        }
        
        public T Emit<T>(T ev) where T : Event {
            listeners.ForEach(listener => listener.Emit(ev));
            return ev;
        }
    }
    
    public class EventListener {
        private readonly IDictionary<Type, List<MethodInfo>> methods = new Dictionary<Type, List<MethodInfo>>();

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
                
                GetHandlerMethods(parameterType).Add(info);
            });
            
            listenerConsumer.Invoke(this);
        }

        public void Emit<T>(T ev) where T: Event {
            GetHandlerMethods(ev.GetType()).ForEach(info => {
                info.Invoke(this, new object[] { ev });
            });
        }
        
        private List<MethodInfo> GetHandlerMethods(Type type) {
            if (!methods.TryGetValue(type, out List<MethodInfo> listeners)) {
                methods[type] = listeners = new List<MethodInfo>();
            }

            return listeners;
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]  
    public class EventHandler : Attribute {}
}