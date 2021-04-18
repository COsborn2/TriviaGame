import {Team} from '@/models.g';

export class Player {

  public connectionId: string;
  public team: Team;
  public buzzerPosition: number | null;

  constructor(connectionId: string, team: Team) {
    this.connectionId = connectionId;
    this.team = team;
    this.buzzerPosition = null;
  }
}
