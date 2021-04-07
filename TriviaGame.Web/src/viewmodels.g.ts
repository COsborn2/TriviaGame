import * as $metadata from './metadata.g'
import * as $models from './models.g'
import * as $apiClients from './api-clients.g'
import { ViewModel, ListViewModel, ServiceViewModel, DeepPartial, defineProps } from 'coalesce-vue/lib/viewmodel'

export interface TriviaBoardViewModel extends $models.TriviaBoard {
  triviaBoardId: number | null;
  question: string | null;
  totalPoints: number | null;
  answers: $models.TriviaAnswer[] | null;
}
export class TriviaBoardViewModel extends ViewModel<$models.TriviaBoard, $apiClients.TriviaBoardApiClient, number> implements $models.TriviaBoard  {
  
  constructor(initialData?: DeepPartial<$models.TriviaBoard> | null) {
    super($metadata.TriviaBoard, new $apiClients.TriviaBoardApiClient(), initialData)
  }
}
defineProps(TriviaBoardViewModel, $metadata.TriviaBoard)

export class TriviaBoardListViewModel extends ListViewModel<$models.TriviaBoard, $apiClients.TriviaBoardApiClient, TriviaBoardViewModel> {
  
  constructor() {
    super($metadata.TriviaBoard, new $apiClients.TriviaBoardApiClient())
  }
}


export class TriviaServiceViewModel extends ServiceViewModel<typeof $metadata.TriviaService, $apiClients.TriviaServiceApiClient> {
  
  public get getRandomTriviaBoard() {
    const getRandomTriviaBoard = this.$apiClient.$makeCaller(
      this.$metadata.methods.getRandomTriviaBoard,
      (c) => c.getRandomTriviaBoard(),
      () => ({}),
      (c, args) => c.getRandomTriviaBoard())
    
    Object.defineProperty(this, 'getRandomTriviaBoard', {value: getRandomTriviaBoard});
    return getRandomTriviaBoard
  }
  
  public get getRandomTriviaBoardWithNoAnswers() {
    const getRandomTriviaBoardWithNoAnswers = this.$apiClient.$makeCaller(
      this.$metadata.methods.getRandomTriviaBoardWithNoAnswers,
      (c) => c.getRandomTriviaBoardWithNoAnswers(),
      () => ({}),
      (c, args) => c.getRandomTriviaBoardWithNoAnswers())
    
    Object.defineProperty(this, 'getRandomTriviaBoardWithNoAnswers', {value: getRandomTriviaBoardWithNoAnswers});
    return getRandomTriviaBoardWithNoAnswers
  }
  
  public get getTriviaBoardOfId() {
    const getTriviaBoardOfId = this.$apiClient.$makeCaller(
      this.$metadata.methods.getTriviaBoardOfId,
      (c, id: number | null) => c.getTriviaBoardOfId(id),
      () => ({id: null as number | null, }),
      (c, args) => c.getTriviaBoardOfId(args.id))
    
    Object.defineProperty(this, 'getTriviaBoardOfId', {value: getTriviaBoardOfId});
    return getTriviaBoardOfId
  }
  
  constructor() {
    super($metadata.TriviaService, new $apiClients.TriviaServiceApiClient())
  }
}


const viewModelTypeLookup = ViewModel.typeLookup = {
  TriviaBoard: TriviaBoardViewModel,
}
const listViewModelTypeLookup = ListViewModel.typeLookup = {
  TriviaBoard: TriviaBoardListViewModel,
}
const serviceViewModelTypeLookup = ServiceViewModel.typeLookup = {
  TriviaService: TriviaServiceViewModel,
}

