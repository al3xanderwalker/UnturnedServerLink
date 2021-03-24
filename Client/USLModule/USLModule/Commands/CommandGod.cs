using SDG.Unturned;
using Steamworks;
using ChatManager = USLModule.Managers.ChatManager;
using PlayerManager = USLModule.Managers.PlayerManager;
#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandGod : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            if (Provider.server.Equals(executorID)) return;
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);

            if (PlayerManager.GodPlayers.Contains(steamPlayer))
            {
                ChatManager.SendServerMessage("You are no longer god", steamPlayer, EChatMode.SAY);
                PlayerManager.GodPlayers.Remove(steamPlayer);
            }
            else
            {
                ChatManager.SendServerMessage("You are now god", steamPlayer, EChatMode.SAY);
                PlayerManager.GodPlayers.Add(steamPlayer);
            }
        }
        public CommandGod()
        {
            localization = new Local(); 
            _command = "god";
            _info = "god";
            _help = "god";
        }
    }
}