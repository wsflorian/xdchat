namespace xdchat_server.Commands {
    public interface ICommandSender {
        void SendMessage(string text);
        string GetName();
    }
}