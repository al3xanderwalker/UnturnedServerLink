export class JoinResponse {
  SteamId: string;
  Banned: boolean;

  constructor(steamId: string, banned: boolean) {
    this.SteamId = steamId;
    this.Banned = banned;
  }
}
