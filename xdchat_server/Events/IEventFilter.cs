namespace xdchat_server.Events {
    public interface IEventFilter {
        bool DoesMatch(object filter);
    }
}