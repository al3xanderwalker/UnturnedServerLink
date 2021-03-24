using SDG.Unturned;
using Steamworks;
using UnityEngine;
using ChatManager = USLModule.Managers.ChatManager;

#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandCheckOwner : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);
            var thingLocated = TraceRay(steamPlayer, 2048f,
                RayMasks.VEHICLE | RayMasks.BARRICADE | RayMasks.STRUCTURE | RayMasks.BARRICADE_INTERACT |
                RayMasks.STRUCTURE_INTERACT);
            if (thingLocated.transform == null)
            {
                ChatManager.SendServerMessage("Could not find barricade", steamPlayer, EChatMode.SAY);
                return;
            }

            var component = thingLocated.transform.GetComponent<Interactable2>();
            
            if (component?.transform == null)
            {
                ChatManager.SendServerMessage("Could not find barricade", steamPlayer, EChatMode.SAY);
                return;
            }
            
            var ownerId = (CSteamID) component.owner;

            ChatManager.SendServerMessage($"Owner: {ownerId}", steamPlayer, EChatMode.SAY);
        }

        public CommandCheckOwner()
        {
            localization = new Local();
            _command = "checkowner";
            _info = "checkowner";
            _help = "checkowner";
        }

        private static RaycastInfo TraceRay(SteamPlayer player, float distance, int masks)
        {
            return DamageTool.raycast(new Ray(player.player.look.aim.position, player.player.look.aim.forward),
                distance, masks);
        }
    }
}