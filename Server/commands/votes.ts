import {Collection, Message} from 'discord.js';
import {Database} from '../models/Database';
import {ExtWebSocket} from '../models/ExtWebSocket';
import {Link} from '../models/Link';
import {USLClient} from '../models/USLClient';
import {Servers} from '../server';
import request from 'request';

export = async (client: USLClient, msg: Message, args: String[]) => {
  var db: Database = await new Database('links').safe();

  if (!client.cooldowns.has('votes'))
    client.cooldowns.set('votes', new Collection());

  var timestamps = client.cooldowns.get('votes');
  var cooldownTime = 10 * 1000;
  var now = Date.now();
  if (timestamps.has(msg.author.id)) {
    const expirationTime = timestamps.get(msg.author.id) + cooldownTime;
    if (now < expirationTime) {
      const timeLeft = (expirationTime - now) / 1000;
      return client.sendErrorEmbed(
        msg,
        'Error',
        `Cooldown: ${timeLeft.toFixed(1)} seconds`
      );
    }
  }
  timestamps.set(msg.author.id, now);
  setTimeout(() => timestamps.delete(msg.author.id), cooldownTime);

  var link: Link = await db.getFirst('Discord', msg.author.id);
  if (!link)
    return client.sendErrorEmbed(
      msg,
      'Account not linked',
      'Link your account using /link in game'
    );

  var serverApiKeys = [];
  for (var key in Servers) {
    var ws: ExtWebSocket = Servers[key];
    serverApiKeys.push(ws.serverInfo.ServerListAPIKey);
  }
  var results = [];
  await Promise.all(
    serverApiKeys.map(async (apiKey) => {
      var result = await new Promise((resolve) => {
        request(
          `https://unturned-servers.net/api/?object=votes&element=claim&key=${apiKey}&steamid=${link.Steam}`,
          {json: true},
          (err, res, body) => {
            if (body === 1)
              request.post({
                headers: {'content-type': 'application/x-www-form-urlencoded'},
                url: `https://unturned-servers.net/api/?action=post&object=votes&element=claim&key=${apiKey}&steamid=${link.Steam}`,
              });
            resolve(body);
          }
        );
      });
      results.push(result);
    })
  );
  var notfound = results.filter((r) => r === 0).length;
  var unclaimed = results.filter((r) => r === 1).length;
  var claimed = results.filter((r) => r === 2).length;

  link.Votes += unclaimed;

  if (link.Votes > 0)
    msg.member.roles.add(
      await msg.member.guild.roles.fetch('693164883180519427')
    );

  await db.set('Steam', link.Steam, link);

  client.sendSuccessEmbed(
    msg,
    `Vote Checking - ${serverApiKeys.length} Servers`,
    `Votes from today:
    **Unclaimed Votes:** ${unclaimed}
  **Claimed Votes:** ${claimed}
  **Not Voted For:** ${notfound}
  
  **Total Votes:** ${link.Votes}`
  );
};
