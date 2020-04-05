using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public class ClientPacketChatMessage : Packet {
        public string Text { get; set; }

        public override string Validate() {
            return !Validation.IsValidMessageText(Text, ' ') ? "Text is invalid" : null;
        }
    }
}