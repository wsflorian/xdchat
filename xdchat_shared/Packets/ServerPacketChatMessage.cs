using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public class ServerPacketChatMessage : Packet {
        public string HashedUuid { get; set; }
        public string Text { get; set; }

        public override string Validate() {
            if (HashedUuid != null && !Validation.IsHex(HashedUuid, 64)) {
                return "HashedUuid is invalid";
            }

            return !Validation.IsValidMessageText(Text, ' ', '\n') ? "Text is invalid" : null;
        }
    }
    
}
