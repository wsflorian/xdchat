using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace xdchat_server.Db {
    public class XdDatabase : DbContext {
        public XdDatabase(DbContextOptions options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DbRank>().HasData(new DbRank {Id = 1, Name = "Admin"});
            modelBuilder.Entity<DbRank>().HasData(new DbRank {Id = 2, Name = "Mod"});
            modelBuilder.Entity<DbRank>().HasData(new DbRank {Id = 3, Name = "Member", IsDefault = true});

            EntityTypeBuilder<DbRankPermission> rankPerm = modelBuilder.Entity<DbRankPermission>();
            rankPerm.HasKey(o => new {o.RankId, o.Permission});
            rankPerm.HasData(DbRankPermission.All(1, "*"));
            rankPerm.HasData(DbRankPermission.All(2, "user.kick", "user.whisper", "server.ping"));
            rankPerm.HasData(DbRankPermission.All(3, "user.whisper", "server.ping"));

            modelBuilder.Entity<DbRoom>().HasData(new DbRoom {Id = 1, Name = "Default", IsDefault = true});
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbUserSession> Sessions { get; set; }
        
        public DbSet<DbRank> Ranks { get; set; }
        public DbSet<DbRankPermission> RankPermissions { get; set; }

        public DbSet<DbRoom> Rooms { get; set; }
        public DbSet<DbMessage> Messages { get; set; }
    }
}