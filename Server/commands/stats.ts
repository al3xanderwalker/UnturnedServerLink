import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {PlayerData} from '../models/PlayerData';
import {USLClient} from '../models/USLClient';
const config = require('config-yml');

export = async (client: USLClient, msg: Message, args: String[]) => {
  var db: Database = await new Database('players').safe();

  var links: Database = await new Database('links').safe();

  var linked = await links.getFirst('Discord', msg.author.id);
  var target: any = linked
    ? args.length
      ? args.join(' ')
      : linked.Steam
    : args.join(' ');

  var playerData: PlayerData = await db.getKeyArray(['ID', 'Name'], target);

  if (!playerData)
    return client.sendErrorEmbed(msg, 'Info:', 'Could not find player');

  var minutes = Math.round(playerData.PlayTime / 1000 / 60);
  var days = Math.floor(minutes / 1440);
  minutes -= days * 1440;
  var hours = Math.floor(minutes / 60);
  minutes -= hours * 60;

  msg.channel.send({
    embed: {
      color: config.color,
      title: `Stats: ${playerData.Name}`,
      description: `**ID:** ${playerData.ID}
      **Kills:** ${playerData.Kills}
      **Deaths:** ${playerData.Deaths}
      **KDR:** ${(playerData.Kills / playerData.Deaths).toFixed(2)}
      **Experience:** ${playerData.Experience}
      **Reputation:** ${playerData.Reputation}
      **Playtime:** ${days} days, ${hours} hours, ${minutes} minutes`,
      thumbnail: {
        url: playerData.Icon,
      },
      footer: {
        text: msg.author.id,
      },
    },
  });
};
