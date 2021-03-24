using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using USLModule.Models;
using USLModule.Utils;
using USLModule.Managers;
using Player = USLModule.Models.Player;

#pragma warning disable 1998
#pragma warning disable 4014

namespace USLModule.Managers
{
    public class PlayerManager : MonoBehaviour, IObjectComponent
    {
        public static readonly List<SteamPlayer> GodPlayers = new List<SteamPlayer>();

        public void Awake()
        {
            CommandWindow.LogError("Player Manager Loaded");
            Provider.onEnemyConnected += OnEnemyConnected;
            Provider.onEnemyDisconnected += OnEnemyDisconnected;
            PlayerLife.onPlayerDied += OnPlayerDied;
            DamageTool.damagePlayerRequested += OnDamagePlayerRequested;
        }

        private static void OnPlayerDied(PlayerLife sender, EDeathCause cause, ELimb limb, CSteamID instigator)
        {
            var killedSteamPlayer = sender.channel.owner;
            var combat = CombatManager.CombatTracker.ContainsKey(killedSteamPlayer) ? CombatManager.CombatTracker[killedSteamPlayer] : null;

            if (combat != null && PlayerTool.getSteamPlayer(instigator) == null &&
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() -
                combat.LastHit <
                Main.Config.CombatExpiration)
                instigator = combat.Aggressor.playerID.steamID;

            var killInfo = new PlayerDeath(killedSteamPlayer, instigator, cause);

            var killer = PlayerTool.getSteamPlayer(instigator);

            if (killer != null)
            {
                killInfo.KillerShotsHit = CombatManager.Accuracies.ContainsKey(killer) ? CombatManager.Accuracies[killer].ShotsHit : 0;
                killInfo.KillerShotsFired = CombatManager.Accuracies.ContainsKey(killer) ? CombatManager.Accuracies[killer].ShotsFired : 0;
                UnityThread.executeCoroutine(CaptureScreen(killInfo, killer));
            }
            else
            {
                var socketData = new SocketData("PlayerDeath", JsonConvert.SerializeObject(killInfo));
                SocketManager.Emit(socketData);
            }
        }

        private static IEnumerator CaptureScreen(PlayerDeath killInfo, SteamPlayer killer)
        {
            killer.player.sendScreenshot(CSteamID.Nil, delegate(CSteamID steamId, byte[] data)
            {
                killInfo.KillerScreenshot = data;
                var socketData = new SocketData("PlayerDeath", JsonConvert.SerializeObject(killInfo));
                SocketManager.Emit(socketData);
            });
            yield return null;
        }

        private static void OnEnemyDisconnected(SteamPlayer steamPlayer)
        {
            var player = new Player(steamPlayer);
            var data = new SocketData("LeavePlayer", JsonConvert.SerializeObject(player));
            SocketManager.Emit(data);
        }

        private static void OnEnemyConnected(SteamPlayer steamPlayer)
        {
            ChatManager.SendServerMessage(Main.Config.WelcomeMessage, steamPlayer, EChatMode.SAY,
                Main.Config.MessageIconUrl, true);
            CombatManager.Accuracies[steamPlayer] = new Accuracy(0, 0);
            CheckBan(steamPlayer);
        }

        private static void OnDamagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (GodPlayers.Contains(parameters.player.channel.owner)) shouldAllow = false;
        }

        private static async Task CheckBan(SteamPlayer steamPlayer)
        {
            await Task.Delay(1000); // Allows for information such as Health to be collected

            var player = new Player(steamPlayer);
            var data = new SocketData("JoinPlayer", JsonConvert.SerializeObject(player));
            SocketManager.Emit(data);
        }

        public static async Task BanPlayer(SteamPlayer executor, string target)
        {
            UnityThread.executeCoroutine(BanPlayerCoroutine(executor, target));
        }

        public static async Task UnbanPlayer(SteamPlayer executor, string target)
        {
            UnityThread.executeCoroutine(UnbanPlayerCoroutine(executor, target));
        }

        private static IEnumerator BanPlayerCoroutine(SteamPlayer executor, string target)
        {
            var ip = 0u;
            if (PlayerTool.tryGetSteamPlayer(target, out var steamPlayer)) ip = steamPlayer.getIPv4AddressOrZero();
            PlayerTool.tryGetSteamID(target, out var cSteamID);

            var steamId = steamPlayer?.playerID.steamID ?? cSteamID;

            if (!steamId.IsValid())
            {
                if (executor != null) ChatManager.SendServerMessage("Target not found", executor, EChatMode.SAY);
                else CommandWindow.LogError("Target not found");
                yield break;
            }

            Provider.requestBanPlayer(Provider.server, steamId, ip, "Banned", SteamBlacklist.PERMANENT);
            var banData = new SocketData("BanPlayer", steamId.ToString());
            SocketManager.Emit(banData);
            ChatManager.SendServerMessage("Banned target successfully", executor, EChatMode.SAY);

            yield return null;
        }

        private static IEnumerator UnbanPlayerCoroutine(SteamPlayer executor, string target)
        {
            PlayerTool.tryGetSteamPlayer(target, out var steamPlayer);
            PlayerTool.tryGetSteamID(target, out var cSteamID);

            var steamId = steamPlayer?.playerID.steamID ?? cSteamID;

            if (!steamId.IsValid())
            {
                if (executor != null) ChatManager.SendServerMessage("Target not found", executor, EChatMode.SAY);
                else CommandWindow.LogError("Target not found");
                yield break;
            }

            Provider.requestUnbanPlayer(Provider.server, steamId);
            var unbanData = new SocketData("UnbanPlayer", steamId.ToString());
            SocketManager.Emit(unbanData);
            ChatManager.SendServerMessage("Unbanned target successfully", executor, EChatMode.SAY);

            yield return null;
        }
    }
}