using System;
using System.Reflection;

namespace XdChatShared.Events {
    public class EventRegistration {
        public EventHandler HandlerInfo { get; }
        public MethodInfo Method { get; }
        public IEventListener Listener { get;  }

        public Type EventType => Method.GetParameters()[0].ParameterType;

        public EventRegistration(EventHandler handlerInfo, MethodInfo method, IEventListener listener) {
            HandlerInfo = handlerInfo;
            Method = method;
            Listener = listener;
        }
    }
}