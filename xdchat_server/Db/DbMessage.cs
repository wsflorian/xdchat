using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xdchat_server.Db {
    public class DbMessage {
        [Key] [Required] public int Id { get; set; }
        
        [Required] public int UserId { get; set; }
        [Required] public DbUser User { get; set; }
        
        [Required] public int RoomId { get; set; }
        [Required] public DbRoom Room { get; set; }
        
        [Required] public DateTime TimeStamp { get; set; }
        [Required] public string Content { get; set; }
    }
}