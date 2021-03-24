using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;

namespace USLModule.Utils
{
    public class Config
    {
        public string WelcomeMessage { get; set; }
        public string MessageIconUrl { get; set; }
        
        public string ChatAnnouncement { get; set; }
        public string AuthorizationToken { get; set; }
        public string ServerId { get; set; }
        public string ChatChannel { get; set; }
        public string DeathLogChannel { get; set; }
        
        public string ServerListID { get; set; }
        
        public string ServerListAPIKey { get; set; }
        
        public byte QueueLimit { get; set; }
        public int SocketTimeout { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public int SaveInterval { get; set; }
        public string DiscordLink { get; set; }
        public int MessageInterval { get; set; }
        public int CombatExpiration { get; set; }
    }
    public class ConfigHelper
    {
        public static void EnsureConfig(string path)
        {
            if (File.Exists(path)) return;
            CommandWindow.LogError("No UnturnedServerLink config found, generating...");

            var pureVanillaConfig = new JObject
            {
                {"WelcomeMessage", "Welcome to our Server!"},
                {"MessageIconUrl", "ChangeMe"},
                {"ChatAnnouncement", "ChangeMe"},
                {"AuthorizationToken", "ChangeMe"},
                {"ServerId", "ChangeMe"},
                {"ChatChannel", "ChangeMe"},
                {"DeathLogChannel", "ChangeMe"},
                {"ServerListID", "ChangeMe"},
                {"ServerListAPIKey", "ChangeMe"},
                {"QueueLimit", 12},
                {"SocketTimeout", 5000},
                {"ServerIp", "0.0.0.0"},
                {"ServerPort", 0},
                {"SaveInterval", 900000},
                {"DiscordLink", "ChangeMe"},
                {"MessageInterval", 1800000},
                {"CombatExpiration", 30000}
            };

            using (var file = File.CreateText(path))
            using (var writer = new JsonTextWriter(file))
            {
                pureVanillaConfig.WriteTo(writer);
                CommandWindow.LogError("Generated UnturnedServerLink config");
            }
        }

        public static Config ReadConfig(string path)
        {
            using (var file = File.OpenText(path))
            using (var reader = new JsonTextReader(file))
            {
                return JsonConvert.DeserializeObject<Config>(JToken.ReadFrom(reader).ToString());
            }
        }
    }
}