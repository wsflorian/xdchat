using System;
using System.Linq;
using XdChatShared;

namespace XdChatShared.Packets {
    public class ClientPacketAuth : Packet {
        public string Uuid { get; set; }
        public string Nickname { get; set; }

        public override string Validate() {
            if (!Guid.TryParse(Uuid, out _)) {
                return "UUID is invalid";
            }

            if (Nickname == null || Nickname.Length > Constants.MaxNickLength || Nickname.Any(e => !char.IsLetterOrDigit(e))) {
                return "Nickname is invalid";
            }

            return null;
        }
    }
}