namespace XdChatShared.Events {
    public interface IEventFilter {
        bool DoesMatch(object filter);
    }
}