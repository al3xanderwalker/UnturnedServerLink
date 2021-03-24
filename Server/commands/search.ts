import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {PlayerData} from '../models/PlayerData';
import {USLClient} from '../models/USLClient';

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (!(await client.checkAdmin(msg))) return;

  var type = args.shift().toString();
  var target = args.join(' ');

  var db: Database = await new Database('players').safe();

  var playerDataArray: PlayerData[] = await db.get(type, target).catch(() => {
    return [];
  });

  if (!playerDataArray || !playerDataArray.length)
    return client.sendErrorEmbed(msg, 'Error', 'Could not find players');

  playerDataArray = playerDataArray.slice(0, 10);

  var resultString = '';

  playerDataArray.forEach((player) => {
    resultString += `**${player.Name}**: \`${player.ID}\`\n`;
  });

  client.sendEmbed(msg, `Search: ${type} = ${target}`, resultString);
};
