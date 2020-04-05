using System;
using XdChatShared.Misc;

namespace XdChatShared.Packets {
    public class ClientPacketAuth : Packet {
        public string Uuid { get; set; }
        public string Nickname { get; set; }

        public override string Validate() {
            if (!Guid.TryParse(Uuid, out _)) {
                return "UUID is invalid";
            }

            if (!Validation.IsValidNickname(Nickname)) {
                return "Nickname is invalid";
            }

            return null;
        }
    }
}