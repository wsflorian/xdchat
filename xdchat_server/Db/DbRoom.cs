using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbRoom {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }
}