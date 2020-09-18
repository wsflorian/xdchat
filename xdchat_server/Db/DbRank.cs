using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using XdChatShared.Misc;

namespace xdchat_server.Db {
    public class DbRank {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        
        public virtual ICollection<DbRankPermission> Permissions { get; set; }

        public bool HasPermission(string comparePerm) {
            return Permissions.Any(perm => Helper.CheckWildcard(perm.Permission, comparePerm));
        }
    }
}