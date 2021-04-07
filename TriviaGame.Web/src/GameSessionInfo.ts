import {TriviaBoard} from "@/models.g";

export class GameSessionInfo {
  constructor() {
    this.gameId = null;
    this.playerIds = [];
    this.triviaBoard = new TriviaBoard();
    this.totalAnswers = 0;
    this.hostId = null;
  }
  
  gameId: string | null;
  playerIds: string[];

  triviaBoard: TriviaBoard;
  
  totalAnswers: number;
  
  hostId: string | null;
}