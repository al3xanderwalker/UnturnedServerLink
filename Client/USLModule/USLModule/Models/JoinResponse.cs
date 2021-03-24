namespace USLModule.Models
{
    public class JoinResponse
    {
        public string SteamId;
        public bool Banned;

        public JoinResponse(string steamId, bool banned)
        {
            SteamId = steamId;
            Banned = banned;
        }
    }
}