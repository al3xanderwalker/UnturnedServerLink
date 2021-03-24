using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using USLModule;
using USLModule.Models;

#pragma warning disable 4014

namespace USLModule.Managers
{
    public class CombatManager : MonoBehaviour, IObjectComponent
    {
        public static Dictionary<SteamPlayer, Accuracy> Accuracies = new Dictionary<SteamPlayer, Accuracy>();
        public static readonly Dictionary<SteamPlayer, Combat> CombatTracker = new Dictionary<SteamPlayer, Combat>();

        public void Awake()
        {
            CommandWindow.LogError("Combat Manager Loaded");
            UseableGun.onBulletSpawned += OnBulletSpawned;
            UseableGun.onBulletHit += OnBulletHit;
        }

        private static void OnBulletSpawned(UseableGun useableGun, BulletInfo bulletInfo) =>
            HitAccuracy(useableGun, bulletInfo);

        private static void OnBulletHit(UseableGun useableGun, BulletInfo bulletInfo, InputInfo hit,
            ref bool shouldAllow)
        {
            if (hit.player) Accuracies[useableGun.player.channel.owner].ShotsHit++;
        }

        private static async Task HitAccuracy(UseableGun useableGun, BulletInfo bulletInfo)
        {
            var steamPlayer = useableGun.player.channel.owner;
            if (!Accuracies.ContainsKey(steamPlayer)) return;
            Accuracies[steamPlayer].ShotsFired++;
            var comparison = Accuracies[steamPlayer].ShotsFired;

            await Task.Delay(Main.Config.CombatExpiration);
            if (comparison == Accuracies[steamPlayer].ShotsFired)
                Accuracies[steamPlayer] = new Accuracy(0, 0);
        }
    }
    
    [HarmonyPatch(typeof(PlayerLife), "doDamage")]
    class DoDamagePatch
    {
        public static void Prefix(byte amount, EDeathCause newCause, ELimb newLimb, CSteamID newKiller, ref EPlayerKill kill, bool trackKill, bool canCauseBleeding, PlayerLife __instance)
        {
            if (PlayerTool.getSteamPlayer(newKiller) == null) return;
            CombatManager.CombatTracker[__instance.channel.owner] =
                new Combat(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), PlayerTool.getSteamPlayer(newKiller));
        }
    }
}