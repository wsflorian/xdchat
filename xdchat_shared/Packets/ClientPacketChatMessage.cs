namespace XdChatShared.Packets {
    public class ClientPacketChatMessage : Packet {
        public string Text { get; set; }

        public override string Validate() {
            return string.IsNullOrEmpty(Text) ? "Text is invalid" : null;
        }
    }
}