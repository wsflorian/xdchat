using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XdChatShared.Packets;

namespace xdchat_server {
    public class Authentication {
        public string Nickname { get; }
        public string Uuid { get; }
        public string HashedUuid { get; }

        public Authentication(string nickname, string uuid) {
            this.Nickname = nickname;
            this.Uuid = uuid;
            this.HashedUuid = Sha256Hash(uuid);
        }

        public ServerPacketClientList.User ToClientListUser() {
            return new ServerPacketClientList.User(Nickname, HashedUuid);
        }
        
        private static string Sha256Hash(string value) {
            using (SHA256 hash = SHA256.Create()) {
                return string.Concat(hash
                    .ComputeHash(Encoding.UTF8.GetBytes(value))
                    .Select(item => item.ToString("x2")));
            }
        } 
    }
}

