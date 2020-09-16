using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using xdchat_shared.Logger.Impl;

namespace xdchat_server.Server {
    public class ServerConfig {
        private const string ConfigFile = @"config.json";
        
        public string SqlConnection { get; set; }
        
        public static ServerConfig load() {
            if (!File.Exists(ConfigFile)) return null;
            string configText = File.ReadAllText(ConfigFile, Encoding.UTF8);
            return JsonConvert.DeserializeObject<ServerConfig>(configText);
        }
        
        public static void create() {
            string configText = JsonConvert.SerializeObject(new ServerConfig() {
                SqlConnection = "server=<name>;userid=<username>;password=<password>;database=<database>;"
            }, Formatting.Indented);
            File.WriteAllText(ConfigFile, configText, Encoding.UTF8);
        }
    }
}