using JetBrains.Annotations;

namespace XdChatShared.Events {
    public interface IEventFilter {
        bool DoesMatch([CanBeNull] object filter);
    }
}