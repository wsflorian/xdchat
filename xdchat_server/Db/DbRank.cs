using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XdChatShared.Misc;

namespace xdchat_server.Db {
    public class DbRank {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
        
        public virtual ICollection<DbRankPermission> Permissions { get; set; }

        public static DbRank GetDefault(XdDatabase db) {
            return db.Ranks.Include(rank => rank.Permissions).Single(rank => rank.IsDefault);
        }
        
        public bool HasPermission(string comparePerm) {
            return Permissions.Any(perm => Helper.CheckWildcard(perm.Permission, comparePerm));
        }
    }
}