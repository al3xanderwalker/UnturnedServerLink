import {Player} from './Player';

export class PlayerData {
  ID: string;
  Hwid: string;
  Name: string;
  IP: string;
  Icon: string;
  GroupID: string;

  LastServer: string;
  FirstJoin: number;
  LastJoin: number;
  PlayTime: number;

  Kills: number;
  Deaths: number;
  Experience: number;
  Reputation: number;

  Banned: boolean;
  VpnOrProxy: boolean;
  PrivacyState: string;
  VacBanned: boolean;
  LimitedAccount: boolean;

  constructor(player: Player, VpnOrProxy: boolean) {
    this.ID = player.ID;
    this.Hwid = player.Hwid;
    this.Name = player.Name;
    this.IP = player.IpAddress;
    this.Icon = player.Icon;
    this.GroupID = player.GroupID;
    this.LastServer = player.ServerName;
    this.FirstJoin = new Date().getTime();
    this.LastJoin = new Date().getTime();
    this.PlayTime = 0;
    this.Kills = 0;
    this.Deaths = 0;
    this.Experience = player.Experience;
    this.Reputation = player.Reputation;
    this.Banned = false;
    this.VpnOrProxy = VpnOrProxy;
    this.PrivacyState = player.PrivacyState;
    this.VacBanned = player.IsVacBanned;
    this.LimitedAccount = player.IsLimitedAccount;
  }
}
