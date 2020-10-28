using Newtonsoft.Json;
using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public class ServerPacketChatMessage : Packet {
        public string HashedUuid { get; set; }
        public string Text { get; set; }

        [JsonIgnore]
        public string CopyText => RelevantRange == null
                ? null
                : Text.Substring(RelevantRange.Value.Item1,RelevantRange.Value.Item2);
        public (int, int)? RelevantRange { get; set; }

        public override string Validate() {
            if (HashedUuid != null && !Validation.IsHex(HashedUuid, 64)) {
                return "HashedUuid is invalid";
            }

            if (!Validation.IsValidMessageText(Text, ' ', '\n')) {
                return "Text is invalid";
            }

            if (RelevantRange != null && !Validation.IsSubstringRange(Text, RelevantRange.Value.Item1, RelevantRange.Value.Item2)) {
                return "CopyRange is invalid";
            }

            return null;
        }
        
    }
    
}
