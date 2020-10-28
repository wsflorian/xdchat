using System;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbWebToken {
        [Key] public int Id { get; set; }
        
        public int UserId { get; set; }
        public virtual DbUser User { get; set; }
        
        public string Token { get; set; }
        public DateTime ExpiryTimeStamp;

        public static DbWebToken Create(XdDatabase db, DbWebToken token) {
            return db.WebTokens.Add(token).Entity;
        }

    }
}