import {ServerInfo} from '../models/ServerInfo';
import {Servers} from '../server';
import {ExtWebSocket} from '../models/ExtWebSocket';

export = async (ws: ExtWebSocket, data) => {
  var serverInfo: ServerInfo = Object.assign(
    new ServerInfo(),
    JSON.parse(data)
  );
  serverInfo.Identifier = serverInfo.Name.slice(1, 4); // `${serverInfo.IP}_${serverInfo.Port}`;
  ws.serverInfo = serverInfo;
  Servers[serverInfo.Identifier] = ws;
};
