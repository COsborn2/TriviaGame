import {TriviaAnswer, TriviaBoard} from '@/models.g';
import {Player, Team} from '@/Player';

export class GameSessionInfo {

  public get totalPointsTeamOne(): number {
    return this.triviaBoard.answers
      ?.filter((value: TriviaAnswer) => value.wonBy === Team.One)
      ?.map((value: TriviaAnswer) => value.points)
      ?.reduce((sum, current) => (sum ?? 0) + (current ?? 0), 0) ?? 0;
  }

  public get totalPointsTeamTwo(): number {
    return this.triviaBoard.answers
      ?.filter((value: TriviaAnswer) => value.wonBy === Team.Two)
      ?.map((value: TriviaAnswer) => value.points)
      ?.reduce((sum, current) => (sum ?? 0) + (current ?? 0), 0) ?? 0;
  }

  public get teamOnePlayers(): Player[] {
    return this.players.filter((value: Player) => value.team === Team.One);
  }

  public get teamTwoPlayers(): Player[] {
    return this.players.filter((value: Player) => value.team === Team.Two);
  }

  public gameId: string | null;

  public players: Player[];

  public triviaBoard: TriviaBoard;

  public totalAnswers: number;

  public host: Player | null;
  constructor() {
    this.gameId = null;
    this.players = [];
    this.triviaBoard = new TriviaBoard();
    this.totalAnswers = 0;
    this.host = null;
  }
}
