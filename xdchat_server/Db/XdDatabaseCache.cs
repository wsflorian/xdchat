using System;
using System.Collections.Generic;
using System.IO;
using xdchat_server.Server;

namespace xdchat_server.Db {
    public class XdDatabaseCache<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly Func<XdDatabase, TKey, TValue> _loader;
        
        public XdDatabaseCache(Func<XdDatabase, TKey, TValue> loader) {
            _loader = loader;
        }
        
        public TValue Get(TKey key) {
            if (_cache.ContainsKey(key)) 
                return _cache[key];

            using (XdDatabase db = XdServer.Instance.Db) {
                return _cache[key] = _loader(db, key);
            }
        }

        public void Remove(TKey key) {
            if (_cache.ContainsKey(key)) {
                _cache.Remove(key);
            }
        }
        
        public void Clear() {
            _cache.Clear();
        }
    }
}