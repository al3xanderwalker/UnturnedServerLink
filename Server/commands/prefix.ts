import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {USLClient} from '../models/USLClient';

export = async (client: USLClient, msg: Message, args: String[]) => {
  var db: Database = await new Database('settings').safe();

  var prefix = await client.getPrefix();

  if (!args[0]) return client.sendEmbed(msg, `The current prefix is ${prefix}`);

  if (!(await client.checkAdmin(msg)))
    return client.sendErrorEmbed(msg, 'Error', 'User lacks permissions');

  await db.setJSON('prefix', args[0]);

  client.sendSuccessEmbed(msg, 'Prefix', `Prefix changed to: \`${args[0]}\``);
};
