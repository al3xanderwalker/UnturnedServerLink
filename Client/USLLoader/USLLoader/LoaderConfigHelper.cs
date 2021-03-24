using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDG.Unturned;

namespace USLLoader
{
    public class Config
    {
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
    }
    public class ConfigHelper
    {
        public static void EnsureConfig(string path)
        {
            if (File.Exists(path)) return;
            CommandWindow.LogError("No Loader config found, generating...");

            var pureVanillaConfig = new JObject
            {
                {"ServerIp", "0.0.0.0"},
                {"ServerPort", 0}
            };

            using (var file = File.CreateText(path))
            using (var writer = new JsonTextWriter(file))
            {
                pureVanillaConfig.WriteTo(writer);
                CommandWindow.LogError("Generated Loader config");
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