using XdChatShared.Events;

namespace XdChatShared {
    public abstract class XdService {
        public EventEmitter EventEmitter { get; } = new EventEmitter();
    }
}