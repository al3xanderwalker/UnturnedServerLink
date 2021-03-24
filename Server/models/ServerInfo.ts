import {Player} from './Player';

export class ServerInfo {
  Identifier: string;
  ID: string;
  ChatChannel: string;
  DeathLogChannel: string;
  ServerListID: string;
  ServerListAPIKey: string;
  Name: string;
  Port: number;
  IP: string;
  Map: string;
  Version: string;
  Players: Player[];
}
