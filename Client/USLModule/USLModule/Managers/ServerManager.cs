using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SDG.Unturned;
using UnityEngine;
using USLModule;
using USLModule.Models;
using USLModule.Utils;

#pragma warning disable 1998

namespace USLModule.Managers
{
    public class ServerManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.LogError("Server Manager Loaded");
            Provider.queueSize = Main.Config.QueueLimit;
            Level.onPostLevelLoaded += (int level) =>
            {
                AsyncHelper.Schedule("AutoSave", SaveServer, Main.Config.SaveInterval);
                AsyncHelper.Schedule("DiscordNotify", AnnounceMessage, Main.Config.MessageInterval);
            };
        }

        private static async Task SaveServer()
        {
            CommandWindow.Log("Saved Server!");
            UnityThread.executeCoroutine(SaveServerCoroutine());
        }

        private static async Task AnnounceMessage()
        {
            await ChatManager.SendServerMessage(Main.Config.ChatAnnouncement, null, EChatMode.GLOBAL);
        }

        private static IEnumerator SaveServerCoroutine()
        {
            if (Level.isLoaded) SaveManager.save();
            yield return null;
        }
    }
}