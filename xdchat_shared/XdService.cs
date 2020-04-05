using XdChatShared.Events;

namespace XdChatShared {
    /* => Services <=
     *
     * => A service represents an application (e.g. Client, Server)
     * => Classes extending XdService should be singletons
     */
    public abstract class XdService {
        public EventEmitter EventEmitter { get; } = new EventEmitter();

        public abstract void Start();
        public abstract void Stop();
    }
}