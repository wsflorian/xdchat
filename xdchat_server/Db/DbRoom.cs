using System.ComponentModel.DataAnnotations;

namespace xdchat_server.Db {
    public class DbRoom {
        [Key] [Required] public int Id { get; set; }
        [Required] public string Name { get; set; }
    }
}