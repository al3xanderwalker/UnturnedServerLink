using SDG.Unturned;
using Steamworks;
using ChatManager = USLModule.Managers.ChatManager;
#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandClearInventory : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);
            
            for (byte page = 0; page < 6; page++)
            {
                for (byte i = 0; i < steamPlayer.player.inventory.items[page].getItemCount(); i++)
                {
                    var item = steamPlayer.player.inventory.items[page].getItem(i);
                    steamPlayer.player.inventory.removeItem(page, steamPlayer.player.inventory.getIndex(page, item.x, item.y));
                }
            }

            void RemoveUnequipped()
            {
                for (byte i = 0; i < steamPlayer.player.inventory.getItemCount(2); i++)
                {
                    steamPlayer.player.inventory.removeItem(2, 0);
                }
            }

            steamPlayer.player.clothing.askWearBackpack(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearGlasses(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearHat(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearPants(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearMask(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearShirt(0, 0, new byte[0], true);
            RemoveUnequipped();
            steamPlayer.player.clothing.askWearVest(0, 0, new byte[0], true);
            RemoveUnequipped();
            
            ChatManager.SendServerMessage("Cleared inventory!", steamPlayer, EChatMode.SAY);
        }
        public CommandClearInventory()
        {
            localization = new Local(); 
            _command = "clearinventory";
            _info = "clearinventory";
            _help = "clearinventory";
        }
    }
}