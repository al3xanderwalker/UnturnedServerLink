import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {ExtWebSocket} from '../models/ExtWebSocket';
import {PlayerData} from '../models/PlayerData';
import {USLClient} from '../models/USLClient';
import {RemoteCommand} from '../models/RemoteCommand';
import {SocketData} from '../models/SocketData';
import {Servers} from '../server';

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (!(await client.checkAdmin(msg))) return;

  var target = args.join(' ');
  var db: Database = await new Database('players').safe();

  var playerData: PlayerData = await db.getKeyArray(
    ['ID', 'Name', 'HWID'],
    target
  );
  if (!playerData)
    return client.sendErrorEmbed(msg, 'GlobalBan', 'Could not find player');

  playerData.Banned = true;
  await db.set('ID', playerData.ID, playerData);

  client.sendSuccessEmbed(msg, `Banned Player: ${playerData.Name}`);

  for (var key in Servers) {
    var ws: ExtWebSocket = Servers[key];
    var remoteCommand = new RemoteCommand(
      msg.author.username,
      `ban ${playerData.ID}`
    );
    ws.sendJSON(new SocketData('RemoteCommand', remoteCommand));
  }

  client.reportBan(playerData, true);
};
