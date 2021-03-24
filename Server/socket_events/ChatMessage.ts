import {ChatMessage} from '../models/ChatMessage';
import {client} from '../server';
import {ExtWebSocket} from '../models/ExtWebSocket';
const config = require('config-yml');

export = async (ws: ExtWebSocket, data) => {
  var chatMessage: ChatMessage = Object.assign(
    new ChatMessage(),
    JSON.parse(data)
  );
  if (!ws.serverInfo) return console.log('ServerInfo Not Found');
  var channel: any = await client.getChannel(ws.serverInfo.ChatChannel);
  if (!channel) return;
  client.sendChatEmbed(
    channel,
    `${chatMessage.Sender}: ${chatMessage.Content}`,
    chatMessage.Icon,
    config.color,
    `http://steamcommunity.com/profiles/${chatMessage.SenderId}/`
  );
};
