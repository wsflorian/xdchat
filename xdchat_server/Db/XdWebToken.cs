using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace xdchat_server.Db {
    public class XdWebToken {
        [Key] public int Id { get; set; }
        
        public int UserId { get; set; }
        public virtual DbUser User { get; set; }
        
        public string Token { get; set; }
        public DateTime ExpiryTimeStamp;

        public static XdWebToken Create(XdDatabase db, XdWebToken token) {
            return db.WebTokens.Add(token).Entity;
        }

    }
}