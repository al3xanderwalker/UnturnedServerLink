import {Database} from '../models/Database';
import {PlayerData} from '../models/PlayerData';
import {ExtWebSocket} from '../models/ExtWebSocket';
import {RemoteCommand} from '../models/RemoteCommand';
import {client, Servers} from '../server';
import {SocketData} from '../models/SocketData';

export = async (ws: ExtWebSocket, data: string) => {
  var db: Database = await new Database('players').safe();

  var target: PlayerData = await db.getFirst('ID', data);
  if (!target) return;
  target.Banned = false;
  await db.set('ID', target.ID, target);

  var remoteCommand = new RemoteCommand('Panel', `unban ${target.ID}`);
  for (var key in Servers) {
    var ws2: ExtWebSocket = Servers[key];
    if (ws2 != ws) ws2.sendJSON(new SocketData('RemoteCommand', remoteCommand));
  }

  client.reportBan(target, false);
};
