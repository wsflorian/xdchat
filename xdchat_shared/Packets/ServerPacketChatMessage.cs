using Newtonsoft.Json;
using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public class ServerPacketChatMessage : Packet {
        public string HashedUuid { get; set; }
        public string Text { get; set; }

        [JsonIgnore]
        public string CopyText => CopyRange == null
                ? null
                : Text.Substring(CopyRange.Value.Item1,CopyRange.Value.Item2);
        public (int, int)? CopyRange { get; set; }

        public override string Validate() {
            if (HashedUuid != null && !Validation.IsHex(HashedUuid, 64)) {
                return "HashedUuid is invalid";
            }

            if (!Validation.IsValidMessageText(Text, ' ', '\n')) {
                return "Text is invalid";
            }

            if (CopyRange != null && !Validation.IsSubstringRange(Text, CopyRange.Value.Item1, CopyRange.Value.Item2)) {
                return "CopyRange is invalid";
            }

            return null;
        }
        
    }
    
}
