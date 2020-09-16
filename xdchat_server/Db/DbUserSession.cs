using System;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbUserSession {
        [Key] [Required] public int Id { get; set; }
        [Required] public DbUser User { get; set; }
        [Required] public DateTime StartTs { get; set; }
        public DateTime EndTs { get; set; }
        [Required] public string Nickname { get; set; }
        [Required] public DbRoom Room { get; set; }
    }
}