import {Message} from 'discord.js';
import {client} from '../server';
import {Database} from './Database';
import {PlayerData} from './PlayerData';

export class LeaderboardEmbed {
  static async updateLeaderboard() {
    var db: Database = await new Database('settings').safe();

    await db.ensure('key', 'leaderboard', JSON.stringify(''));

    var playerDb: Database = await new Database('players').safe();

    var kills: PlayerData[] = await playerDb.getTop5('Kills');
    var experience: PlayerData[] = await playerDb.getTop5('Experience');
    var reputation: PlayerData[] = await playerDb.getTop5('Reputation');
    if (!kills || !experience || !reputation) return;

    var response = client.embed('Live Leaderboard:');
    response.embed.fields = [
      client.newField('**Most Kills:**'),
      client.newField('**Most Reputation:**'),
      client.newField('**Most Experience:**'),
    ];
    if (!kills[4]) return;
    for (var i = 0; i < 5; i++) {
      var ii = i + 1;
      response.embed.fields.push(
        client.newField(`**${ii}. ${kills[i].Name}**\n${kills[i].Kills} Kills`),
        client.newField(
          `**${ii}. ${reputation[i].Name}**\n${reputation[i].Reputation} Reputation`
        ),
        client.newField(
          `**${ii}. ${experience[i].Name}**\n${experience[i].Experience} Experience`
        )
      );
    }
    var info = await db.getJSON('leaderboard');
    if (!info) return;
    var channel: any = await client.getChannel(info.channel);
    if (!channel) return;
    var message: Message = await client.getMessage(channel, info.message);
    if (!message) return;
    message.edit(response);
  }
}
