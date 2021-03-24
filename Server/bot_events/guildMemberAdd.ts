import {GuildMember} from 'discord.js';
import {USLClient} from '../models/USLClient';
const config = require('config-yml');

module.exports = async (client: USLClient, member: GuildMember) => {
  if (config.autoRoleId == '') return;
  member.roles.add(await member.guild.roles.fetch(config.autoRoleId));
};
