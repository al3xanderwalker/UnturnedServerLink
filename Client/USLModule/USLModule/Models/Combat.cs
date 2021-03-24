using SDG.Unturned;

namespace USLModule.Models
{
    public class Combat
    {
        public long LastHit;
        public SteamPlayer Aggressor;

        public Combat(long lastHit, SteamPlayer steamPlayer)
        {
            LastHit = lastHit;
            Aggressor = steamPlayer;
        }
    }
}