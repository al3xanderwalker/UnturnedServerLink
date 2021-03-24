export class PlayerDeath {
  KilledSteamId: string;
  KilledName: string;

  KillerSteamId: string;
  KillerName: string;
  KillerWeapon: string;
  KillerShotsHit: number;
  KillerShotsFired: number;
  KillerScreenshot: Uint8Array;

  Cause: string;
  Distance: number;
}
