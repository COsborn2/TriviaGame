import * as metadata from './metadata.g'
import { Model, DataSource, convertToModel, mapToModel } from 'coalesce-vue/lib/model'

export interface TriviaBoard extends Model<typeof metadata.TriviaBoard> {
  triviaBoardId: number | null
  question: string | null
  totalPoints: number | null
  answers: TriviaAnswer[] | null
}
export class TriviaBoard {
  
  /** Mutates the input object and its descendents into a valid TriviaBoard implementation. */
  static convert(data?: Partial<TriviaBoard>): TriviaBoard {
    return convertToModel(data || {}, metadata.TriviaBoard) 
  }
  
  /** Maps the input object and its descendents to a new, valid TriviaBoard implementation. */
  static map(data?: Partial<TriviaBoard>): TriviaBoard {
    return mapToModel(data || {}, metadata.TriviaBoard) 
  }
  
  /** Instantiate a new TriviaBoard, optionally basing it on the given data. */
  constructor(data?: Partial<TriviaBoard> | {[k: string]: any}) {
      Object.assign(this, TriviaBoard.map(data || {}));
  }
}


export interface TriviaAnswer extends Model<typeof metadata.TriviaAnswer> {
  triviaAnswerId: number | null
  answer: string | null
  points: number | null
  triviaBoard: TriviaBoard | null
  position: number | null
}
export class TriviaAnswer {
  
  /** Mutates the input object and its descendents into a valid TriviaAnswer implementation. */
  static convert(data?: Partial<TriviaAnswer>): TriviaAnswer {
    return convertToModel(data || {}, metadata.TriviaAnswer) 
  }
  
  /** Maps the input object and its descendents to a new, valid TriviaAnswer implementation. */
  static map(data?: Partial<TriviaAnswer>): TriviaAnswer {
    return mapToModel(data || {}, metadata.TriviaAnswer) 
  }
  
  /** Instantiate a new TriviaAnswer, optionally basing it on the given data. */
  constructor(data?: Partial<TriviaAnswer> | {[k: string]: any}) {
      Object.assign(this, TriviaAnswer.map(data || {}));
  }
}


