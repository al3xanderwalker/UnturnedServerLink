import {Message} from 'discord.js';
import {Database} from '../models/Database';
import {USLClient} from '../models/USLClient';
const config = require('config-yml');

export = async (client: USLClient, msg: Message, args: String[]) => {
  if (msg.author.id != config.ownerId)
    return client.sendErrorEmbed(msg, 'Error', 'User lacks permissions');

  var db: Database = await new Database('settings').safe();

  var option = args.shift();
  var target = args[0] ? args[0].toString() : null;

  if (!option) return client.sendErrorEmbed(msg, 'Error', `Invalid Usage`);

  var admins: string[] = await db.getJSON('admins');

  switch (option) {
    case 'list':
      var adminList = '';
      admins.forEach((admin) => (adminList += `\`${admin}\`, `));

      client.sendEmbed(msg, 'admins', adminList);
      break;

    case 'add':
      if (!target) return client.sendErrorEmbed(msg, 'Error', `Invalid Usage`);

      if (!admins.includes(target)) admins.push(target);

      await db.setJSON('admins', admins);
      client.sendSuccessEmbed(msg, 'Admins', `Added admin: \`${target}\``);
      break;

    case 'remove':
      if (!target) return client.sendErrorEmbed(msg, 'Error', `Invalid Usage`);

      admins = admins.filter((a) => a != target);

      await db.setJSON('admins', admins);
      client.sendSuccessEmbed(msg, 'Admins', `Removed admin: \`${target}\``);
      break;
  }
};
