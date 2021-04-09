import {TriviaBoard} from "@/models.g";
import {Player, Team} from "@/Player";

export class GameSessionInfo {
  constructor() {
    this.gameId = null;
    this.players = [];
    this.triviaBoard = new TriviaBoard();
    this.totalAnswers = 0;
    this.host = null;
  }

  gameId: string | null;
  
  players: Player[];

  triviaBoard: TriviaBoard;

  totalAnswers: number;

  host: Player | null;

  public get teamOnePlayers(): Player[] {
    return this.players.filter((value: Player) => value.team === Team.Left);
  }

  public get teamTwoPlayers(): Player[] {
    return this.players.filter((value: Player) => value.team === Team.Right);
  }
}