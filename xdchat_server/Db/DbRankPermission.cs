﻿using System.Collections.Generic;
using System.Linq;

namespace xdchat_server.Db {

    public class DbRankPermission {
        public int RankId { get; set; }
        public virtual DbRank Rank { get; set; } 
        public string Permission { get; set; }

        public static List<DbRankPermission> All(int rankId, params string[] permissions) {
            return permissions.Select(perm => new DbRankPermission {RankId = rankId, Permission = perm}).ToList();
        }

        public override bool Equals(object obj)
        {
            var rankPerm = obj as DbRankPermission;
            return rankPerm != null && (this.Permission == rankPerm.Permission && this.RankId == rankPerm.RankId);
        }
    }
}