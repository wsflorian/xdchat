using XdChatShared;

namespace XdChatShared.Packets {
    public class ServerPacketDisconnect : Packet {
        public string Text { get; set; }

        public override string Validate() {
            return Validation.IsAlphaNumeric(Text) ? null : "Text is invalid";
        }
    }
}