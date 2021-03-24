using SDG.Unturned;
using Steamworks;
using ChatManager = USLModule.Managers.ChatManager;
#pragma warning disable 4014

namespace USLModule.Commands
{
    public class CommandVanish : Command
    {
        protected override void execute(CSteamID executorID, string parameter)
        {
            if (Provider.server.Equals(executorID)) return;
            var steamPlayer = PlayerTool.getSteamPlayer(executorID);
            
            var look = steamPlayer.player.look;
            var movement = steamPlayer.player.movement;
            var vanished = !movement.canAddSimulationResultsToUpdates;
            if (vanished)
            {
                ChatManager.SendServerMessage("You are no longer vanished", steamPlayer, EChatMode.SAY);
                movement.updates.Add(new PlayerStateUpdate(movement.real, look.angle, look.rot));
            }
            else
            {
                ChatManager.SendServerMessage("You are now vanished", steamPlayer, EChatMode.SAY);
            }

            steamPlayer.player.movement.canAddSimulationResultsToUpdates = vanished;

        }
        
        public CommandVanish()
        {
            localization = new Local(); 
            _command = "vanish";
            _info = "vanish";
            _help = "vanish";
        }
    }
}