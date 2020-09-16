using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbUser {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Uuid { get; set; }
        [Required] public DbRank Rank { get; set; }
    }
}