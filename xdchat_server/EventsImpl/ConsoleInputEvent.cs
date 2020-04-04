using XdChatShared.Events;

namespace xdchat_server.EventsImpl {
    public class ConsoleInputEvent : Event {
        public string Input { get; }

        public ConsoleInputEvent(string input) {
            Input = input;
        }
    }
}