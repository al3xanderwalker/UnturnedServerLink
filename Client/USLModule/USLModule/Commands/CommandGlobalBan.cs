using SDG.Unturned;
using Steamworks;
using PlayerManager = USLModule.Managers.PlayerManager;
#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandGlobalBan : Command
    {
        protected override void execute(CSteamID executor, string parameter)
        {
            var ply = PlayerTool.getSteamPlayer(executor);
            PlayerManager.BanPlayer(ply, parameter);
        }

        public CommandGlobalBan()
        {
            this.localization = new Local();
            this._command = "globalban";
            this._info = "globalban";
            this._help = "globalban player";
        }
    }
}