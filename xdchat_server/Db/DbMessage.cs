using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace xdchat_server.Db {
    public class DbMessage {
        [Key] public int Id { get; set; }
        
        public int UserId { get; set; }
        public DbUser User { get; set; }
        
        public int RoomId { get; set; }
        public DbRoom Room { get; set; }
        
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }

        public static void Insert(XdDatabase db, DbRoom room, DbUser user, string message) {
            db.Messages.Add(new DbMessage {
                Room = room,
                User = user,
                TimeStamp = DateTime.Now,
                Content = message
            });
        }

        public static List<DbMessage> GetRecent(XdDatabase db, int roomId, int count) {
            return db.Messages.Include(msg => msg.User)
                .Where(msg => msg.RoomId == roomId)
                .OrderByDescending(msg => msg.TimeStamp)
                .Take(count)
                .OrderBy(msg => msg.TimeStamp)
                .ToList();
        }
    }
}