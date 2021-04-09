export class Player {
  constructor(connectionId: string, team: Team) {
    this.connectionId = connectionId;
    this.team = team;
  }

  connectionId: string;
  team: Team;
}

export enum Team {
  Unknown = 0,
  Left = 1,
  Right = 2
}