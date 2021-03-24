import {Client, Collection, Message} from 'discord.js';
import {appRoot, Servers} from '../server';
import requireAll from 'require-all';
import {Database} from './Database';
import {RemoteCommand} from './RemoteCommand';
import {SocketData} from './SocketData';
import {ChatMessage} from './ChatMessage';
import {ExtWebSocket} from './ExtWebSocket';
import {ServerInfoEmbed} from './ServerInfoEmbed';
import {LeaderboardEmbed} from './LeaderboardEmbed';
import {PlayerData} from './PlayerData';
const config = require('config-yml');

export class USLClient extends Client {
  commands: Map<String, Function>;
  cooldowns: Collection<string, Collection<string, number>>;

  sendEmbed(
    msg: Message,
    title: string,
    description?: string,
    color: string = config.color
  ) {
    msg.channel.send(this.embed(title, description, color));
  }

  sendErrorEmbed = (msg: Message, title: string, description: string = '') =>
    this.sendEmbed(msg, title, description, '#E04646');

  sendSuccessEmbed = (msg: Message, title: string, description: string = '') =>
    this.sendEmbed(msg, title, description, '#3EBD2F');

  chatEmbed = (
    name: string,
    icon: string,
    color = config.color,
    url: string = ''
  ) => ({
    embed: {
      color: color,
      author: {
        name: name,
        icon_url: icon,
        url: url,
      },
    },
  });

  sendChatEmbed = async (
    channel: any,
    name: string,
    icon: string,
    color = config.color,
    url: string = ''
  ) => channel.send(this.chatEmbed(name, icon, color, url));

  embed = (
    title: string,
    description?: string,
    color: string = config.color
  ) => ({
    embed: {title: title, description: description, color: color, fields: []},
  });

  loadEvents() {
    const events = requireAll({
      dirname: `${appRoot}/bot_events`,
      filter: /^(?!-)(.+)\.js$/,
    });
    this.removeAllListeners();
    for (const name in events) {
      const event = events[name];
      this.on(name, event.bind(null, this));
      console.log(`Event loaded: ${name}`);
    }
  }

  loadCommands() {
    const commands = requireAll({
      dirname: `${appRoot}/commands`,
      filter: /^(?!-)(.+)\.js$/,
    });
    this.commands = new Map();
    for (const name in commands) {
      const cmd = commands[name];
      this.commands.set(name, cmd);
      console.log(`Command loaded: ${name}`);
    }
  }

  getPrefix = async () => {
    var db: Database = await new Database('settings').safe();
    await db.ensureJSON('prefix', ';');
    return db.getJSON('prefix');
  };

  newField = (value: string) => ({
    name: '___________',
    value: value,
    inline: true,
  });

  checkAdmin = async (msg: Message) => {
    var db: Database = await new Database('settings').safe();
    await db.ensureJSON('admins', []);

    let admins = await db.getJSON('admins');
    if (!admins.includes(msg.author.id)) {
      msg.channel.send(
        this.embed('Command Failed', 'User lacks permissions', '#E04646')
      );
      return false;
    }
    return true;
  };

  async chatLink(message: Message) {
    for (var key in Servers) {
      var ws: ExtWebSocket = Servers[key];
      if (ws.serverInfo.ChatChannel == message.channel.id) {
        if (message.content[0] == '/') {
          var db: Database = await new Database('settings').safe();
          await db.ensureJSON('admins', []);

          if (!this.checkAdmin(message)) return;

          var remoteCommand = new RemoteCommand(
            message.author.username,
            message.content.substring(1)
          );
          ws.sendJSON(new SocketData('RemoteCommand', remoteCommand));
          message.delete({timeout: 100});
          message.channel.send(
            this.chatEmbed(
              `${message.author.username} executed ${message.content}`,
              message.author.displayAvatarURL(),
              '#2CF5F7'
            )
          );
          return;
        }
        var chatMessage = new ChatMessage(
          message.author.username,
          message.author.id,
          message.content,
          message.author.displayAvatarURL({format: 'png'})
        );
        ws.sendJSON(new SocketData('ChatMessage', chatMessage));
        message.delete({timeout: 100});
        message.channel.send(
          this.chatEmbed(
            `${message.author.username}: ${message.content}`,
            message.author.displayAvatarURL(),
            '#6277C3'
          )
        );
      }
    }
  }

  async getChannel(id: string) {
    return await this.channels.fetch(id).catch(() => {
      return;
    });
  }

  async getMessage(channel: any, id: string) {
    return await channel.messages.fetch(id).catch(() => {
      return;
    });
  }

  reportDisconnect = async (ws: ExtWebSocket) => {
    var channel: any = await this.getChannel(ws.serverInfo.ChatChannel);
    if (!channel) return;
    this.sendChatEmbed(
      channel,
      `Disconnected from ${ws.serverInfo.Name}`,
      config.iconUrl,
      '#E04646'
    );
  };

  reportBan = async (target: PlayerData, banned: boolean) => {
    var settings: Database = await new Database('settings').safe();
    await settings.ensureJSON('banLogs', '');
    var banLogs = await settings.getJSON('banLogs');
    var channel: any = await this.getChannel(banLogs.channel);
    if (!channel) return;
    channel.send({
      embed: {
        color: banned ? '#E04646' : '#3EBD2F',
        title: `${banned ? 'Banned' : 'Unbanned'} Player`,
        description: `Target: ${target.Name} (${target.ID})`,
        thumbnail: {
          url: target.Icon,
        },
      },
    });
  };

  initialize() {
    this.cooldowns = new Collection();
    this.loadEvents();
    this.loadCommands();
    this.login(config.botToken)
      .then(() => {
        console.log('Logged in');
        // @ts-ignore-start
        this.api.applications(this.user.id).commands.post({
          data: {
            name: 'test',
            description: 'test command',
          },
        });
        // @ts-ignore-end
        setInterval(() => {
          ServerInfoEmbed.updateServerInfo();
          LeaderboardEmbed.updateLeaderboard();
        }, 30000);
      })
      .catch(() => console.log('Failed to log in'));
  }
}
