using JetBrains.Annotations;

namespace xdchat_server.Commands {
    public interface ICommandSender {
        void SendMessage([NotNull] string text);
        [NotNull] string GetName();
    }
}