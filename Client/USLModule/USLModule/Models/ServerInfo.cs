using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using SDG.Unturned;

namespace USLModule.Models
{
    public class ServerInfo
    {
        public string ID;
        public string ChatChannel;
        public string DeathLogChannel;
        public string ServerListID;
        public string ServerListAPIKey;
        public string Name;
        public ushort Port;
        public string IP;
        public string Map;
        public string Version;
        public List<Player> Players = new List<Player>();

        public ServerInfo(string name, ushort port, string map, string version, List<SteamPlayer> clients)
        {
            ID = Main.Config.ServerId;
            ChatChannel = Main.Config.ChatChannel;
            DeathLogChannel = Main.Config.DeathLogChannel;
            ServerListID = Main.Config.ServerListID;
            ServerListAPIKey = Main.Config.ServerListAPIKey;
            Name = name;
            Port = port;
            try
            {
                IP = new StreamReader(
                    WebRequest.CreateHttp("http://icanhazip.com/").GetResponse().GetResponseStream() ??
                    new MemoryStream(Encoding.UTF8.GetBytes("0.0.0.0"))).ReadToEnd().Replace("\n", "");
            }
            catch
            {
                IP = "0.0.0.0";
            }
            Map = map;
            Version = version;
            clients.ForEach(p => Players.Add(new Player(p)));
        }
    }
}