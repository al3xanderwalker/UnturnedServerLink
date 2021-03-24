using SDG.Unturned;
using UnityEngine;
using USLModule.Commands;
using USLModule.Models;

namespace USLModule.Managers
{
    public class CommandManager : MonoBehaviour, IObjectComponent
    {
        public void Awake()
        {
            CommandWindow.LogError("Command Manager Loaded");
            SDG.Unturned.ChatManager.onCheckPermissions += OnCheckPermissions;
            
            Commander.register(new CommandDiscord());
            Commander.register(new CommandGlobalBan());
            Commander.register(new CommandGlobalUnban());
            Commander.register(new CommandToggleCheats());
            Commander.register(new CommandCheckOwner());
            Commander.register(new CommandLink());
            Commander.register(new CommandVanish());
            Commander.register(new CommandGod());
            Commander.register(new CommandClearInventory());
        }

        public void OnCheckPermissions(SteamPlayer steamPlayer, string text, ref bool shouldExecuteCommand,
            ref bool shouldList)
        {
            if (text.StartsWith("/discord") || text.StartsWith("/link"))
            {
                shouldExecuteCommand = true;
            }
        }
    }
}