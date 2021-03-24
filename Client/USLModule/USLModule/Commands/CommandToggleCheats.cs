using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace USLModule.Commands
{
    public class CommandToggleCheats : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var ply = PlayerTool.getSteamPlayer(executorID);
            Provider.hasCheats = !Provider.hasCheats;
            var state = Provider.hasCheats ? "Enabled" : "Disabled";
            if (Provider.server != executorID)
            {
                ChatManager.serverSendMessage($"Cheats: {state}", Color.white, null, ply, EChatMode.SAY, "https://i.imgur.com/tcZ3u3R.png", true);
            }
            CommandWindow.Log($"Cheats: {state}");
        }

        public CommandToggleCheats()
        {
            this.localization = new Local();
            this._command = "togglecheats";
            this._info = "togglecheats";
            this._help = "toggle cheats";
        }
    }
}