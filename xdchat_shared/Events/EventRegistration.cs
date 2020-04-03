using System;
using System.Reflection;
using JetBrains.Annotations;

namespace XdChatShared.Events {
    public class EventRegistration {
        [NotNull] public EventHandler HandlerInfo { get; }
        [NotNull] public MethodInfo Method { get; }
        [NotNull] public IEventListener Listener { get; }

        [NotNull] public Type EventType => Method.GetParameters()[0].ParameterType;

        public EventRegistration([NotNull] EventHandler handlerInfo, [NotNull] MethodInfo method, [NotNull] IEventListener listener) {
            HandlerInfo = handlerInfo;
            Method = method;
            Listener = listener;
        }
    }
}