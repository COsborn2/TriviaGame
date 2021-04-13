export class Player {

  public connectionId: string;
  public team: Team;
  constructor(connectionId: string, team: Team) {
    this.connectionId = connectionId;
    this.team = team;
  }
}

export enum Team {
  Unknown = 0,
  One = 1,
  Two = 2,
}
