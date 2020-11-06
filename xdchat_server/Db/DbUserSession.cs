using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace xdchat_server.Db {
    public class DbUserSession {
        [Key] public int Id { get; set; }
        
        public int UserId { get; set; }
        public virtual DbUser User { get; set; }
        public DateTime StartTs { get; set; }
        public DateTime? EndTs { get; set; }
        public string Nickname { get; set; }
        
        public int RoomId { get; set; }
        public virtual DbRoom Room { get; set; }

        public static DbUserSession Create(XdDatabase db, DbUserSession session) {
            return db.Sessions.Add(session).Entity;
        }

        public static void EndSession(XdDatabase db, DbUserSession session) {
            session.EndTs = DateTime.Now;
            db.Sessions.Update(session);
        }

        public static DbUserSession GetById(XdDatabase db, int id) {
            return db.Sessions
                .Include(s => s.Room)
                .Include(s => s.User)
                .FirstOrDefault(session => session.Id == id);
        }

        public static void Update(XdDatabase db, DbUserSession session) {
            db.Sessions.Update(session);
        }
    }
}