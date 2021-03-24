using System;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace USLModule.Models
{
    public class PlayerDeath
    {
        public string KilledSteamId;
        public string KilledName;

        public string KillerSteamId;
        public string KillerName;
        public string KillerWeapon;
        public int KillerShotsHit = 0;
        public int KillerShotsFired = 0;
        public byte[] KillerScreenshot;

        public string Cause;
        public float Distance = 0f;

        public PlayerDeath(SteamPlayer killed, CSteamID killerId, EDeathCause deathCause)
        {
            Cause = Enum.GetName(typeof(EDeathCause), deathCause)?.ToLower();
            KilledSteamId = killed.playerID.steamID.ToString();
            KilledName = killed.playerID.playerName;
            var killer = PlayerTool.getSteamPlayer(killerId);
            if (killer == null) return;
            if (killer.Equals(killed)) return;
            
            KillerSteamId = killer.playerID.steamID.ToString();
            KillerName = killer.playerID.playerName;
            Distance = Vector3.Distance(killed.player.transform.position, killer.player.transform.position);

            if (killer.player.equipment == null) return;
            if (killer.player.equipment.asset != null) KillerWeapon = killer.player.equipment.asset.name;

        }
    }
}