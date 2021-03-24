import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {Link} from '../models/Link';
import {USLClient} from '../models/USLClient';

export = async (client: USLClient, msg: Message, args: String[]) => {
  var prefix = await client.getPrefix();

  var db: Database = await new Database('links').safe();

  var code = args[0] ? args[0].toString() : null;

  if (!code)
    return client.sendErrorEmbed(
      msg,
      'Usage:',
      `**1.** Generate your code in game using \`/link\`\n**2.** Use \`${prefix}link <Code>\` in discord with the code generated in game`
    );

  var link: Link = await db.getFirst('Code', code);
  if (!link || link.Discord) return client.sendErrorEmbed(msg, 'Invalid Code');

  link.Discord = msg.author.id;
  await db.set('Steam', link.Steam, link);

  client.sendSuccessEmbed(msg, 'Account Linked');
};
