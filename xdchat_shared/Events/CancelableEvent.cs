namespace XdChatShared.Events {
    public class CancelableEvent : Event {
        public bool Cancelled { get; set; } = false;
    }
}