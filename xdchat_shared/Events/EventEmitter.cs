using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using xdchat_shared.Logger.Impl;
using XdChatShared.Modules;
using XdChatShared.Scheduler;

namespace XdChatShared.Events {
    public class EventEmitter {
        private readonly Dictionary<IEventListener, List<EventRegistration>> _listeners = new Dictionary<IEventListener, List<EventRegistration>>();

        public void RegisterListener([NotNull] IEventListener listener) {
            if (_listeners.ContainsKey(listener)) {
                throw new InvalidOperationException("Event listener already registered");
            }
            
            List<EventRegistration> registrations = _listeners[listener] = new List<EventRegistration>();
            
            listener.GetType().GetMethods().ToList().ForEach(info => {
                object[] attributes = info.GetCustomAttributes(typeof(XdEventHandler), false);
                
                if (attributes.Length < 1) return;
                if (info.GetParameters().Length < 1) {
                    XdLogger.Warn("EventHandler methods require at least one parameter");
                    return;
                }

                Type parameterType = info.GetParameters()[0].ParameterType;
                if (!parameterType.IsSubclassOf(typeof(Event))) {
                    XdLogger.Warn("EventHandler parameter must be of type Event");
                    return;
                }
                
                registrations.Add(new EventRegistration((XdEventHandler) attributes[0], info, listener));
            });
        }
        
        public T Emit<T>([NotNull] T ev) where T : Event {
            XdScheduler.CheckIsMainThread();
            
            _listeners
                .SelectMany(keyValue => keyValue.Value)
                .Where(registration => registration.EventType == ev.GetType())
                .Where(registration => { /* Event filters */
                    if (registration.HandlerInfo.Filter == null) return true; // Not filter set

                    if (!(ev is IEventFilter filter)) {
                        XdLogger.Warn($"Event method '{registration.Method.Name}' has event filter, but event cannot be filtered");
                        return true; // Event is not filterable
                    }

                    return filter.DoesMatch(registration.HandlerInfo.Filter);
                })
                .Where(r => {
                    if (!r.HandlerInfo.ContextScoped) return true; // Not scoped to module
                    
                    IAnonymousContextProvider moduleContextProvider = r.Listener as IAnonymousContextProvider;
                    if (moduleContextProvider == null) {
                        XdLogger.Warn($"Event method is scoped to context, but listener doesn't provide context (not a module?)");
                        return true;
                    }

                    if (!(ev is IAnonymousContextProvider eventContextProvider)) {
                        XdLogger.Warn($"Event method is scoped to context, but event doesn't provide context");
                        return true;
                    }

                    return moduleContextProvider.AnonymousModuleContext == eventContextProvider.AnonymousModuleContext;
                })
                .ToList()
                .ForEach(registration => {
                    registration.Method.Invoke(registration.Listener, new object[] {ev});
                });
            
            return ev;
        }
    }
}