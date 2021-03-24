import {SocketData} from '../models/SocketData';
import {client} from '../server';
import {ExtWebSocket} from '../models/ExtWebSocket';
const config = require('config-yml');

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export = async (ws: ExtWebSocket, data: string) => {
  if (data != config.authToken) {
    ws.sendJSON(new SocketData('Close', ''));
    await sleep(1000);
    return ws.close();
  }
  console.log('\u001b[32mClient Connected\u001b[37m');
  ws.sendJSON(new SocketData('ServerInfo', ''));

  setTimeout(async () => {
    if (!ws.serverInfo) return;
    var channel: any = await client.getChannel(ws.serverInfo.ChatChannel);
    if (!channel) return;

    client.sendChatEmbed(
      channel,
      `Connected to ${ws.serverInfo.Name}`,
      config.iconUrl,
      '#3EBD2F'
    );
  }, 1000);
};
