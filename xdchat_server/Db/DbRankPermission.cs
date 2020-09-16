using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace xdchat_server.Db {

    public class DbRankPermission {
        [Required] public int RankId { get; set; }
        [Required] public DbRank Rank { get; set; } 
        [Required] public string Permission { get; set; }

        public static List<DbRankPermission> All(int rankId, params string[] permissions) {
            return permissions.Select(perm => new DbRankPermission {RankId = rankId, Permission = perm}).ToList();
        }
    }
}