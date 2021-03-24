import {ServerInfo} from './ServerInfo';
import WebSocket from 'ws';

export interface ExtWebSocket extends WebSocket {
  serverInfo: ServerInfo;
  sendJSON: Function;
}
