import {PlayerDeath} from '../models/PlayerDeath';
import {PlayerData} from '../models/PlayerData';
import {MessageAttachment} from 'discord.js';
import {appRoot, client} from '../server';
import {Database} from '../models/Database';
import {writeFile} from 'fs';
import {ExtWebSocket} from '../models/ExtWebSocket';
const config = require('config-yml');

export = async (ws: ExtWebSocket, data) => {
  var playerDeath: PlayerDeath = Object.assign(
    new PlayerDeath(),
    JSON.parse(data)
  );
  var db: Database = await new Database('players').safe();

  var killed: PlayerData = await db.getFirst('ID', playerDeath.KilledSteamId);
  if (!killed)
    return console.log('Could not killed data: ' + playerDeath.KilledSteamId);
  killed.Deaths++;
  await db.set('ID', killed.ID, killed);

  if (playerDeath.KillerSteamId == null) return;

  var killer: PlayerData = await db.getFirst('ID', playerDeath.KillerSteamId);
  if (!killer)
    return console.log(
      'Could not find killer data: ' + playerDeath.KillerSteamId
    );
  killer.Kills++;
  await db.set('ID', killer.ID, killer);

  var imageId = `${new Date().getTime()}_${playerDeath.KilledSteamId}`;

  writeFile(
    `${appRoot}/spy_images/${imageId}.png`,
    playerDeath.KillerScreenshot,
    'base64',
    async (err) => {
      if (err) console.log(err);

      const attachment = new MessageAttachment(
        `./spy_images/${imageId}.png`,
        `${imageId}.png`
      );

      if (!ws.serverInfo) return console.log('ServerInfo Not Found');
      var channel: any = await client.getChannel(ws.serverInfo.DeathLogChannel);
      if (!channel) return;

      channel.send({
        embed: {
          color: config.color,
          title: 'Deathlog',
          description: `Killed: [**${
            playerDeath.KilledName
          }**](http://steamcommunity.com/profiles/${
            playerDeath.KilledSteamId
          }) (${playerDeath.KilledSteamId})
          Killer: [**${
            playerDeath.KillerName
          }**](http://steamcommunity.com/profiles/${
            playerDeath.KillerSteamId
          }) (${playerDeath.KillerSteamId})
          Weapon: **${
            playerDeath.KillerWeapon ? playerDeath.KillerWeapon : 'Fist'
          }**
          Distance: **${playerDeath.Distance.toFixed(1)}m**
          Death Cause: **${
            playerDeath.Cause.charAt(0).toUpperCase() +
            playerDeath.Cause.slice(1)
          }**
          Accuracy: **${playerDeath.KillerShotsHit}/${
            playerDeath.KillerShotsFired
          }**`,
          files: [attachment],
          image: {
            url: `attachment://${imageId}.png`,
          },
        },
      });
    }
  );
};
