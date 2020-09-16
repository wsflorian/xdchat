using Microsoft.EntityFrameworkCore;

namespace xdchat_server.Db {
    public class XdDatabase : DbContext {
        public XdDatabase(DbContextOptions options) : base(options) {
        }
        
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbUserSession> Sessions { get; set; }
        
        public DbSet<DbRank> Ranks { get; set; }
        public DbSet<DbRankPermission> RankPermissions { get; set; }

        public DbSet<DbRoom> Rooms { get; set; }
        public DbSet<DbMessage> Messages { get; set; }
    }
}