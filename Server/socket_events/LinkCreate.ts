import {Database} from '../models/Database';
import {Link} from '../models/Link';
import {SocketData} from '../models/SocketData';
import {ExtWebSocket} from '../models/ExtWebSocket';

var chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890'.split(
  ''
);

export = async (ws: ExtWebSocket, data) => {
  var db: Database = await new Database('links').safe();

  var link: Link = await db.getFirst('Steam', data);
  if (link) {
    ws.sendJSON(new SocketData('LinkResponse', link));
    return;
  }
  link = new Link();
  link.Steam = data;
  link.Discord = '';
  link.Code = '';
  var unique = false;
  while (!unique) {
    for (var i = 0; i < 3; i++)
      link.Code += chars[Math.floor(Math.random() * chars.length)];
    if (!await db.getFirst('Code', link.Code)) unique = true;
  }

  await db.set('Steam', link.Steam, link);
  ws.sendJSON(new SocketData('LinkResponse', link));
};
