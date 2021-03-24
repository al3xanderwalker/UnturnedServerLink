using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using USLModule.Models;
using USLModule.Utils;
using SDG.Unturned;
using UnityEngine;
using WatsonWebsocket;

#pragma warning disable 4014
#pragma warning disable 1998

namespace USLModule.Managers
{
    public class SocketManager : MonoBehaviour, IObjectComponent
    {
        private static WatsonWsClient _client;
        private static long _ack;
        private static bool _attemptingConnection;
        private static bool _forceClosed;
        private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public static async Task Emit(SocketData socketData)
        {
            if (_client?.Connected == true) _client.SendAsync(JsonConvert.SerializeObject(socketData));
        }

        public void Awake()
        {
            CommandWindow.LogError("Socket Manager Loaded");
            Level.onPostLevelLoaded += (int level) =>
            {
                _ack = Now();
                AsyncHelper.Schedule("ack", AckTask, 1000);
            };
        }

        private static async Task StartConnection()
        {
            if (_attemptingConnection || _forceClosed) return;

            _attemptingConnection = true;

            _client = new WatsonWsClient(Main.Config.ServerIp, Main.Config.ServerPort, false);
            _client.ServerConnected += (sender, eventArgs) =>
            {
                _attemptingConnection = false;
                _ack = Now();
            };
            _client.ServerDisconnected += (sender, eventArgs) =>
            {
                _attemptingConnection = false;
                StartConnection();
            };
            _client.MessageReceived += (sender, args) => MessageReceived(sender, args);
            _client.Start();
        }

        private static async Task AckTask()
        {
            if (_client?.Connected == true) _client.SendAsync("ack");

            else if (Now() - _ack > Main.Config.SocketTimeout)
            {
                StartConnection();
            }
        }

        private static async Task MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            try
            {
                if (Encoding.UTF8.GetString(args.Data) == "ack")
                {
                    _ack = Now();
                    return;
                }

                var socketData = JsonConvert.DeserializeObject<SocketData>(Encoding.UTF8.GetString(args.Data));
                switch (socketData.Event)
                {
                    case "Close":
                        _forceClosed = true;
                        _client.Dispose();
                        CommandWindow.LogError("Connection Disposed Successfully");
                        break;
                    case "Authorization":
                        var authorizationToken = new SocketData("Authorization", Main.Config.AuthorizationToken);
                        Emit(authorizationToken);
                        break;
                    case "JoinResponse":
                        var response = JsonConvert.DeserializeObject<JoinResponse>(socketData.Data);
                        if (response.Banned)
                            PlayerManager.BanPlayer(PlayerTool.getSteamPlayer(Provider.server), response.SteamId);
                        else UnityThread.executeCoroutine(BrowserRequest(response.SteamId));
                        break;
                    case "ServerInfo":
                        var data = new ServerInfo(Provider.serverName, Provider.port, Provider.map,
                            Provider.APP_VERSION, Provider.clients.ToList());
                        var newSocketData = new SocketData("ServerInfo", JsonConvert.SerializeObject(data));
                        Emit(newSocketData);
                        break;
                    case "RemoteCommand":
                        var remoteCommand = JsonConvert.DeserializeObject<RemoteCommand>(socketData.Data);
                        UnityThread.executeCoroutine(ExecuteRemoteCommand(remoteCommand));
                        break;
                    case "ChatMessage":
                        var remoteChat = JsonConvert.DeserializeObject<ChatMessage>(socketData.Data);
                        ChatManager.DiscordToServer(remoteChat);
                        break;
                    case "LinkResponse":
                        var link = JsonConvert.DeserializeObject<Link>(socketData.Data);
                        if (!PlayerTool.tryGetSteamPlayer(link.Steam, out var steamPlayer)) return;
                        ChatManager.SendServerMessage(
                            link.Discord != ""
                                ? "You have already linked your account"
                                : $"Use ?link {link.Code} in discord!", steamPlayer, EChatMode.SAY);
                        break;
                    default:
                        CommandWindow.LogError($"Unexpected Event Received: {socketData.Event}");
                        break;
                }
            }
            catch (Exception ex)
            {
                CommandWindow.LogError(ex);
            }
        }

        private static IEnumerator BrowserRequest(string steamId)
        {
            PlayerTool.tryGetSteamPlayer(steamId, out var steamPlayer);
            steamPlayer?.player.sendBrowserRequest("Join our discord!", Main.Config.DiscordLink);
            yield return null;
        }

        public static IEnumerator ExecuteRemoteCommand(RemoteCommand command)
        {
            CommandWindow.Log($"{command.Sender} executed {command.Command}");
            Commander.execute(Provider.server, command.Command);
            yield return null;
        }
    }
}