using System;
using System.Reflection;
using JetBrains.Annotations;

namespace XdChatShared.Events {
    public class EventRegistration {
        [NotNull] public XdEventHandler HandlerInfo { get; }
        [NotNull] public MethodInfo Method { get; }
        [NotNull] public IEventListener Listener { get; }

        [NotNull] public Type EventType => Method.GetParameters()[0].ParameterType;

        public EventRegistration([NotNull] XdEventHandler handlerInfo, [NotNull] MethodInfo method, [NotNull] IEventListener listener) {
            HandlerInfo = handlerInfo;
            Method = method;
            Listener = listener;
        }
    }
}