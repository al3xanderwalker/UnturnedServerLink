import WebSocket from 'ws';
import {SocketData} from './models/SocketData';
import requireAll from 'require-all';
import {USLClient} from './models/USLClient';
import path from 'path';
import {ExtWebSocket} from './models/ExtWebSocket';
import express from 'express';
import {scheduleJob} from 'node-schedule';
import {Database} from './models/Database';
export var appRoot = path.resolve(__dirname);

const config = require('config-yml');

export var Servers = {};
export var client: USLClient = new USLClient();

express()
  .get('/module', (req, res) =>
    res.sendFile(`${__dirname}/USLModule.dll`, 'USLModule.dll')
  )
  .listen(config.modulePort);

new WebSocket.Server({port: config.websocketPort}).on(
  'connection',
  (ws: ExtWebSocket) => {
    ws.sendJSON = (data) => ws.send(JSON.stringify(data));

    ws.sendJSON(new SocketData('Authorization', ''));

    ws.on('message', (message: string) => {
      if (message == 'ack') return ws.send('ack');

      var socketData = Object.assign(
        new SocketData('', ''),
        JSON.parse(message)
      );
      const events = requireAll({
        dirname: `${__dirname}/socket_events`,
        filter: /^(?!-)(.+)\.js$/,
      });
      events[socketData.Event](ws, socketData.Data);
    });

    ws.on('close', () => ConnectionClosedHandler(ws));

    ws.on('error', () => ConnectionClosedHandler(ws));
  }
);

var ConnectionClosedHandler = (ws: ExtWebSocket) => {
  console.log('\u001b[31mClient Disconnected\u001b[37m');
  for (var key in Servers) {
    if (Servers[key] == ws) delete Servers[key];
  }
  client.reportDisconnect(ws);
};

setInterval(() => {
  for (var key in Servers) {
    var ws: ExtWebSocket = Servers[key];
    ws.sendJSON(new SocketData('ServerInfo', ''));
  }
}, 30000);

console.log('Server Started');

client.initialize();

scheduleJob('30 * * * *', async () => {
  var db = await new Database('statistics').safe();
  var now = new Date().getTime().toString();
  var data = {};
  for (var key in Servers) {
    var server: ExtWebSocket = Servers[key];
    data[key] = server.serverInfo.Players.length;
  }
  db.setJSON(now, data);
});
