using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbRankPermission {
        [Required] public DbRank Rank;
        [Required] public string Permission;
    }
}