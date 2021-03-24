using SDG.Unturned;
using Steamworks;

namespace USLModule.Commands
{
    public class CommandDiscord : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);
            steamPlayer?.player.sendBrowserRequest("Join our discord!", Main.Config.DiscordLink);
        }

        public CommandDiscord()
        {
            localization = new Local(); 
            _command = "discord";
            _info = "discord";
            _help = "discord link";
        }
    }
}