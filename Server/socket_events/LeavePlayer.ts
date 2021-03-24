import {ExtWebSocket} from '../models/ExtWebSocket';
import {Player} from '../models/Player';

export = async (ws: ExtWebSocket, data) => {
  var player: Player = Object.assign(new Player(), JSON.parse(data));
  await player.UpdatePlaytime();
  await player.UpdatePlayer();
};
