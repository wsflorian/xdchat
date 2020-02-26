namespace xdchat_server.Events {
    public class Event {}

    public class CancelableEvent : Event {
        public bool Cancelled { get; set; } = false;
    }

    public interface IEventFilter {
        bool DoesMatch(string filter);
    }
}