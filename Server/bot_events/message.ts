import {Message} from 'discord.js';
import {USLClient} from '../models/USLClient';

module.exports = async (client: USLClient, message: Message) => {
  var prefix = await client.getPrefix();

  if (message.author.bot || message.channel.type == 'dm') return;
  if (!message.content.startsWith(prefix)) return client.chatLink(message);

  const [cmd, ...args] = message.content
    .trim()
    .slice(prefix.length)
    .split(/\s+/g);
  const command = client.commands.get(cmd);
  if (command) {
    command(client, message, args);
  }
};
