using XdChatShared;

namespace XdChatShared.Packets {
    public class ServerPacketDisconnect : Packet {
        public string Text { get; set; }

        public override string Validate() {
            return Text != null && Text.Length < 256 ? null : "Text is invalid";
        }
    }
}