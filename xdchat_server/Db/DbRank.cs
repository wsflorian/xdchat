using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbRank {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
        
        public virtual ICollection<DbRankPermission> Permissions { get; set; }
    }
}