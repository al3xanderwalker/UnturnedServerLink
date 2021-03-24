import {PlayerData} from './PlayerData';
import {Database} from './Database';

const util = require('util');
const request = require('request');
const requestPromise = util.promisify(request);

export class Player {
  ID: string;
  Hwid: string;
  Name: string;
  PrivacyState: string;
  IpAddress: string;
  IsVacBanned: boolean;
  IsLimitedAccount: boolean;
  Icon: string;
  Health: number;
  Water: number;
  Food: number;
  Experience: number;
  Reputation: number;
  ServerName: string;
  GroupID: string;

  Sanitize() {
    if (this.Name == '') this.Name = 'Idiot';
    if (!this.Icon) this.Icon = 'None';
    if (!this.PrivacyState) this.PrivacyState = 'None';
  }
  async UpdatePlayer() {
    this.Sanitize();
    var db: Database = await new Database('players').safe();

    var oldPlayerData: PlayerData = await db.getFirst('ID', this.ID);
    if (!oldPlayerData) {
      var VpnOrProxy = false;
      var vpnCheck = await requestPromise(
        `https://ip.teoh.io/api/vpn/${this.IpAddress}`
      );
      try {
        if (JSON.parse(vpnCheck.body).vpn_or_proxy == 'yes') VpnOrProxy = true;
      } catch {
        VpnOrProxy = false;
      }
      var playerData = new PlayerData(this, VpnOrProxy);
      await db.set('ID', playerData.ID, playerData);
    } else {
      var playerData = new PlayerData(this, oldPlayerData.VpnOrProxy);
      if (playerData.IP == '0.0.0.0') playerData.IP = oldPlayerData.IP;
      playerData.FirstJoin = oldPlayerData.FirstJoin;
      playerData.Kills = oldPlayerData.Kills;
      playerData.Deaths = oldPlayerData.Deaths;
      playerData.Banned = oldPlayerData.Banned;
      playerData.PlayTime = oldPlayerData.PlayTime;
      await db.set('ID', playerData.ID, playerData);
    }
  }
  async GetInfo() {
    this.Sanitize();
    var db: Database = await new Database('players').safe();

    var playerData: PlayerData = await db.getFirst('ID', this.ID);
    return playerData;
  }
  async UpdatePlaytime() {
    var db: Database = await new Database('players').safe();

    var playerData: PlayerData = await db.getFirst('ID', this.ID);
    if (!playerData) return;
    var difference = new Date().getTime() - playerData.LastJoin;
    playerData.PlayTime += difference;
    await db.set('ID', playerData.ID, playerData);
  }
}
