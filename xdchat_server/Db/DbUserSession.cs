using System;
using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbUserSession {
        [Key] public int Id { get; set; }
        public DbUser User { get; set; }
        public DateTime StartTs { get; set; }
        public DateTime? EndTs { get; set; }
        public string Nickname { get; set; }
        public DbRoom Room { get; set; }

        public static DbUserSession Create(XdDatabase db, DbUserSession session) {
            return db.Sessions.Add(session).Entity;
        }

        public static void EndSession(XdDatabase db, DbUserSession session) {
            session.EndTs = DateTime.Now;
            db.Sessions.Update(session);
        }
    }
}