using System;
using System.Net;
using System.Xml;
using SDG.Unturned;
using SteamGameServerNetworkingUtils = SDG.Unturned.SteamGameServerNetworkingUtils;

namespace USLModule.Models
{
    public class Player
    {
        public string ID;
        public string Hwid;
        public string Name;
        public string PrivacyState;
        public string IpAddress;
        public bool IsVacBanned;
        public bool IsLimitedAccount;
        public Uri Icon;
        public byte Health;
        public byte Water;
        public byte Food;
        public uint Experience;
        public int Reputation;
        public string ServerName;
        public string GroupID;

        public Player(SteamPlayer steamPlayer)
        {

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(new WebClient().DownloadString("http://steamcommunity.com/profiles/" +
                                                           steamPlayer.playerID.steamID + "?xml=1"));
                Icon = doc["profile"]?["avatarFull"]?.ParseUri();
                PrivacyState = doc["profile"]?["privacyState"]?.ParseString();
                IsVacBanned = doc["profile"]?["vacBanned"]?.ParseUInt32() == 1;
                IsLimitedAccount = doc["profile"]?["isLimitedAccount"]?.ParseUInt32() == 1;
            }
            catch
            {
                Icon = new Uri("https://opengameart.org/sites/default/files/Transparency500.png");
                PrivacyState = "None";
                IsVacBanned = false;
                IsLimitedAccount = false;
            }
            IpAddress = UInt32ToIPAddress(SteamGameServerNetworkingUtils.getIPv4AddressOrZero(steamPlayer.playerID.steamID)).ToString();
            if (IpAddress == "0.0.0.0") IpAddress = UInt32ToIPAddress(steamPlayer.getIPv4AddressOrZero()).ToString();
            ID = steamPlayer.playerID.steamID.ToString();
            Hwid = string.Join("-", steamPlayer.playerID.hwid);
            GroupID = steamPlayer.player.quests.groupID.ToString();
            Name = steamPlayer.playerID.playerName;
            Health = steamPlayer.player.life.health;
            Water = steamPlayer.player.life.water;
            Food = steamPlayer.player.life.food;
            Experience = steamPlayer.player.skills.experience;
            Reputation = steamPlayer.player.skills.reputation;
            ServerName = Main.ServerName;
        }
        private static IPAddress UInt32ToIPAddress(uint address)
        {
            return new IPAddress(new byte[] {
                (byte)((address>>24) & 0xFF) ,
                (byte)((address>>16) & 0xFF) ,
                (byte)((address>>8)  & 0xFF) ,
                (byte)( address & 0xFF)});
        }
    }
    public static class XmlElementExtensions
    {
        public static string ParseString(this XmlElement element)
        {
            return element.InnerText;
        }

        public static uint? ParseUInt32(this XmlElement element)
        {
            try
            {
                return element == null ? null : (uint?)uint.Parse(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Uri ParseUri(this XmlElement element)
        {
            try
            {
                return element == null ? null : new Uri(element.InnerText);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}