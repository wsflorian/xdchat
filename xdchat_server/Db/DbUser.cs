﻿using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace xdchat_server.Db {
    public class DbUser {
        [Key] public int Id { get; set; }
        public string Uuid { get; set; }
        
        //public int RankId { get; set; }
        public virtual DbRank Rank { get; set; }
        
        public virtual DbUserSession LastSession { get; set; }
        public int LastSessionId { get; set; }

        public static DbUser GetByUuid(XdDatabase db, string uuid) {
            return db.Users
                .Include(a => a.Rank)
                .Include(a => a.Rank.Permissions)
                .FirstOrDefault(a => a.Uuid == uuid);
        }
        
        public static DbUser Create(XdDatabase db, string uuid) {
            return db.Users.Add(new DbUser {
                Uuid = uuid,
                Rank = DbRank.GetDefault(db)
            }).Entity;
        }

        public static void Update(XdDatabase db, DbUser user) {
            db.Users.Update(user);
        }

        public static void SetLastSession(XdDatabase db, DbUser user, DbUserSession session) {
            user.LastSessionId = session.Id;
            db.Users.Update(user);
        }
    }
}