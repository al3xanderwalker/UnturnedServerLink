import {Message} from 'discord.js';
import {client, Servers} from '../server';
import {Database} from './Database';
import {ServerInfo} from './ServerInfo';

export class ServerInfoEmbed {
  static async updateServerInfo() {
    var db: Database = new Database('settings');
    await db.ensureTable();
    await db.ensure('key', 'serverInfo', JSON.stringify(''));

    var total = 0,
      totalMax = 0,
      description = '';
    for (var key in Servers) {
      var info: ServerInfo = Servers[key].serverInfo;
      total += info.Players.length;
      totalMax += 24;
      description += `**${info.Name}** - [**${info.Players.length}/24**] [[**Vote**](https://unturned-servers.net/server/${info.ServerListID}/vote/)]
          **IP: \`${info.IP}\` **Port:** \`${info.Port}\` **Map: \`${info.Map}\`\n\n`;
    }
    var serverInfo = await db.getJSON('serverInfo');
    if (!serverInfo) return;
    var channel: any = await client.getChannel(serverInfo.channel);
    if (!channel) return;
    var message: Message = await client.getMessage(channel, serverInfo.message);
    if (!message) return;
    message.edit(
      client.embed(`Server Status - [**${total}/${totalMax}**]`, description)
    );
  }
}
