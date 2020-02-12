using System.Collections.Generic;
using System.Linq;

namespace XdChatShared.Packets {
    public class ServerPacketClientList : Packet {
        public List<User> Users { get; set; }
        
        public override string Validate() {
            if (Users == null) {
                return "Users is invalid";
            }

            string usersError = Validation.GetFirstError(Users);
            return usersError != null ? $"Users entry is invalid: {usersError}" : null;
        }
        
        public class User : IValidatable {
            public string Nickname { get; set; }
            public string HashedUuid { get; set; }

            public User(string nickname, string hashedUuid) {
                Nickname = nickname;
                HashedUuid = hashedUuid;
            }
            
            public string Validate() {
                if (!Validation.IsHex(HashedUuid, 64)) {
                    return "HashedUuid is invalid";
                }

                return !Validation.IsValidNickname(Nickname) ? "Nickname is invalid" : null;
            }
        }
    }
}