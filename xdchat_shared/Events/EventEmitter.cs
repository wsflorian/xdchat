﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleLogger;
using XdChatShared.Modules;
using XdChatShared.Scheduler;

namespace XdChatShared.Events {
    public class EventEmitter {
        private readonly Dictionary<IEventListener, List<EventRegistration>> _listeners = new Dictionary<IEventListener, List<EventRegistration>>();

        public void RegisterListener(IEventListener listener) {
            if (_listeners.ContainsKey(listener)) {
                throw new InvalidOperationException("Event listener already registered");
            }
            
            List<EventRegistration> registrations = _listeners[listener] = new List<EventRegistration>();
            
            listener.GetType().GetMethods().ToList().ForEach(info => {
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
                
                registrations.Add(new EventRegistration((EventHandler) attributes[0], info, listener));
            });
        }
        
        public T Emit<T>(T ev) where T : Event {
            XdScheduler.CheckIsMainThread();
            
            _listeners
                .SelectMany(keyValue => keyValue.Value)
                .Where(registration => registration.EventType == ev.GetType())
                .Where(registration => { /* Event filters */
                    if (registration.HandlerInfo.Filter == null) return true; // Not filter set
                    
                    IEventFilter filter = ev as IEventFilter;
                    if (filter == null) {
                        Logger.Log(Logger.Level.Warning, $"Event method '{registration.Method.Name}' has event filter, but event cannot be filtered");
                        return true; // Event is not filterable
                    }

                    return filter.DoesMatch(registration.HandlerInfo.Filter);
                })
                .Where(r => {
                    if (!r.HandlerInfo.ContextScoped) return true; // Not scoped to module
                    
                    IAnonymousContextProvider moduleContextProvider = r.Listener as IAnonymousContextProvider;
                    if (moduleContextProvider == null) {
                        Logger.Log(Logger.Level.Warning, $"Event method is scoped to context, but listener doesn't provide context (not a module?)");
                        return true;
                    }

                    IAnonymousContextProvider eventContextProvider = ev as IAnonymousContextProvider;
                    if (eventContextProvider == null) {
                        Logger.Log(Logger.Level.Warning, $"Event method is scoped to context, but event doesn't provide context");
                        return true;
                    }

                    return moduleContextProvider.AnonymousModuleContext == eventContextProvider.AnonymousModuleContext;
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