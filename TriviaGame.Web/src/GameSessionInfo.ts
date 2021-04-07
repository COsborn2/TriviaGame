import {TriviaBoard} from "@/models.g";

export interface GameSessionInfo {
  gameId: string | null;
  playerIds: string[];

  triviaBoard: TriviaBoard;
  
  totalAnswers: number;
}