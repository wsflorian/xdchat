using System;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbMessage {
        [Key] public int Id { get; set; }
        
        public int UserId { get; set; }
        public DbUser User { get; set; }
        
        public int RoomId { get; set; }
        public DbRoom Room { get; set; }
        
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }
    }
}