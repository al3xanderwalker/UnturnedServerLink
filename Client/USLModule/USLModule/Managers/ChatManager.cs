using System.Collections;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SDG.Unturned;
using UnityEngine;
using USLModule.Managers;
using USLModule.Models;
using USLModule.Utils;
using Player = USLModule.Models.Player;

#pragma warning disable 1998
#pragma warning disable 4014

namespace USLModule.Managers
{
    public class ChatManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.LogError("Chat Manager Loaded");
            SDG.Unturned.ChatManager.onChatted += OnChatted;
        }

        public void Sleep() =>
            SDG.Unturned.ChatManager.onChatted -= OnChatted;

        private static void OnChatted(SteamPlayer steamPlayer, EChatMode mode, ref Color chatted, ref bool isRich,
            string text, ref bool isVisible)
        {
            if (mode == EChatMode.GLOBAL && text[0] != '/') ServerToDiscord(steamPlayer, text);
        }

        private static async Task ServerToDiscord(SteamPlayer steamPlayer, string text)
        {
            var player = new Player(steamPlayer);
            var message = new ChatMessage(player.Name, player.ID, text, player.Icon.ToString());
            var data = new SocketData("ChatMessage", JsonConvert.SerializeObject(message));
            SocketManager.Emit(data);
        }
        
        public static async Task SendServerMessage(string message, SteamPlayer target, EChatMode chatMode, string iconURL = "https://i.imgur.com/tcZ3u3R.png", bool richText = false)
        {
            UnityThread.executeCoroutine(SendServerMessageCoroutine(message, target, chatMode, iconURL, richText));
        }

        public static async Task DiscordToServer(ChatMessage chatMessage)
        {
            SendServerMessage($"[<color=#899AD3>Discord</color>]{chatMessage.Sender}: {chatMessage.Content}",
                null, EChatMode.GLOBAL, chatMessage.Icon, true);
            CommandWindow.Log($"[Discord] {chatMessage.Sender}: \"{chatMessage.Content}\"");
        }
        
        private static IEnumerator SendServerMessageCoroutine(string message, SteamPlayer target, EChatMode chatMode, string iconURL, bool richText)
        {
            SDG.Unturned.ChatManager.serverSendMessage(message, Color.white, null, target, chatMode, iconURL, richText);
            yield return null;
        }
    }
}