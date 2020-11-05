using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace xdchat_server.Server {
    public class ServerConfig {
        private const string ConfigFile = @"config.json";
        
        public string SqlConnection { get; set; }
        
        public string ServerHost { get; set; }
        public ushort ServerPort { get; set; }
        
        public bool TlsEnabled { get; set; }
        public string TlsCertFile { get; set; }
        
        public static ServerConfig Load() {
            if (!File.Exists(ConfigFile)) return null;
            string configText = File.ReadAllText(ConfigFile, Encoding.UTF8);
            return JsonConvert.DeserializeObject<ServerConfig>(configText);
        }
        
        public static void Create() {
            string configText = JsonConvert.SerializeObject(new ServerConfig() {
                SqlConnection = "server=<name>;userid=<username>;password=<password>;database=<database>;",
                ServerHost = "127.0.0.1",
                ServerPort = 22222,
                TlsEnabled = false,
                TlsCertFile = null
            }, Formatting.Indented);
            File.WriteAllText(ConfigFile, configText, Encoding.UTF8);
        }
    }
}