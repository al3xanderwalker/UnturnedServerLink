import {Player} from '../models/Player';
import {JoinResponse} from '../models/JoinResponse';
import {SocketData} from '../models/SocketData';
import {PlayerData} from '../models/PlayerData';
import {Database} from '../models/Database';
import {ExtWebSocket} from '../models/ExtWebSocket';

export = async (ws: ExtWebSocket, data) => {
  var player: Player = Object.assign(new Player(), JSON.parse(data));
  await player.UpdatePlayer();
  var playerData = await player.GetInfo();

  var db: Database = await new Database('players').safe();

  var playerDataArray: PlayerData[] = await db.get('HWID', playerData.Hwid);

  var responseData = new JoinResponse(
    player.Name,
    playerDataArray.some((e) => e.Banned) || playerData.Banned
  );
  ws.sendJSON(new SocketData('JoinResponse', responseData));
};
