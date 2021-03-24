using SDG.Unturned;
using Steamworks;
using USLModule.Managers;
using USLModule.Models;

#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandLink : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);
            var socketData = new SocketData("LinkCreate", steamPlayer.playerID.steamID.ToString());
            SocketManager.Emit(socketData);
        }

        public CommandLink()
        {
            localization = new Local(); 
            _command = "link";
            _info = "link";
            _help = "link";
        }
    }
}