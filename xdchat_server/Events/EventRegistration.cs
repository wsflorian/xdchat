using System.Reflection;

namespace xdchat_server.Events {
    public class EventRegistration {
        public EventHandler HandlerInfo { get; }
        public MethodInfo Method { get; }
        public EventListener Listener { get;  }

        public EventRegistration(EventHandler handlerInfo, MethodInfo method, EventListener listener) {
            HandlerInfo = handlerInfo;
            Method = method;
            Listener = listener;
        }
    }
}