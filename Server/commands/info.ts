import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {PlayerData} from '../models/PlayerData';
import {USLClient} from '../models/USLClient';
const config = require('config-yml');

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (!(await client.checkAdmin(msg))) return;

  var target = args.join(' ');
  var db: Database = await new Database('players').safe();

  var playerData: PlayerData = await db.getKeyArray(
    ['ID', 'Name', 'HWID'],
    target
  );
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
      title: `Stats: ${playerData.Name} - ${playerData.ID}`,
      description: `**ID:** ${playerData.ID}
      **HWID:** ${playerData.Hwid}
      **GroupID:** ${playerData.GroupID}

      **Kills:** ${playerData.Kills}
      **Deaths:** ${playerData.Deaths}
      **KDR:** ${(playerData.Kills / playerData.Deaths).toFixed(2)}
      **Experience:** ${playerData.Experience}
      **Reputation:** ${playerData.Reputation}
      **Playtime:** ${days} days, ${hours} hours, ${minutes} minutes

      **Last Server:** ${playerData.LastServer}
      **First Played:** ${new Date(playerData.FirstJoin).toDateString()}
      **Last Played:** ${new Date(playerData.LastJoin).toDateString()}
      **Banned:** ${playerData.Banned ? 'True' : 'False'}
      **VPN/Proxy:** ${playerData.VpnOrProxy ? 'True' : 'False'}
      **Profile Privacy:** ${playerData.PrivacyState}
      **VAC Banned:** ${playerData.VacBanned ? 'True' : 'False'}
      **Limited Account:** ${playerData.LimitedAccount ? 'True' : 'False'}`,
      thumbnail: {
        url: playerData.Icon,
      },
    },
  });
};
