import {Message} from 'discord.js';
import {USLClient} from '../models/USLClient';
import {Servers} from '../server';
import {SocketData} from '../models/SocketData';
import {RemoteCommand} from '../models/RemoteCommand';
import {ChatMessage} from '../models/ChatMessage';
import {ExtWebSocket} from '../models/ExtWebSocket';

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (!(await client.checkAdmin(msg))) return;

  var data: SocketData;
  var payload = args.join(' ');
  if (payload[0] == '/') {
    var remoteCommand = new RemoteCommand(
      msg.author.username,
      payload.substring(1)
    );
    data = new SocketData('RemoteCommand', remoteCommand);
  } else {
    var chatMessage = new ChatMessage(
      msg.author.username,
      msg.author.id,
      payload,
      msg.author.displayAvatarURL({format: 'png'})
    );
    data = new SocketData('ChatMessage', chatMessage);
  }
  for (var key in Servers) {
    var ws: ExtWebSocket = Servers[key];
    ws.sendJSON(data);
  }

  client.sendSuccessEmbed(msg, 'Success', `Sent \`${payload}\` to all servers`);
};
