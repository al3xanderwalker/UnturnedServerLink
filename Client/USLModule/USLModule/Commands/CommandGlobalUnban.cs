using SDG.Unturned;
using Steamworks;
using PlayerManager = USLModule.Managers.PlayerManager;
#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandGlobalUnban : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            var ply = PlayerTool.getSteamPlayer(executor);
            PlayerManager.UnbanPlayer(ply, parameter);
        }

        public CommandGlobalUnban()
        {
            this.localization = new Local();
            this._command = "globalunban";
            this._info = "globalunban";
            this._help = "globalunban player";
        }
    }
}