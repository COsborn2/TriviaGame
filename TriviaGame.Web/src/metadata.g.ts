import {
  Domain, getEnumMeta, solidify, ModelType, ObjectType,
  PrimitiveProperty, ForeignKeyProperty, PrimaryKeyProperty,
  ModelCollectionNavigationProperty, ModelReferenceNavigationProperty
} from 'coalesce-vue/lib/metadata'


const domain: Domain = { enums: {}, types: {}, services: {} }
export const TriviaBoard = domain.types.TriviaBoard = {
  name: "TriviaBoard",
  displayName: "Trivia Board",
  get displayProp() { return this.props.triviaBoardId }, 
  type: "model",
  controllerRoute: "TriviaBoard",
  get keyProp() { return this.props.triviaBoardId }, 
  behaviorFlags: 0,
  props: {
    triviaBoardId: {
      name: "triviaBoardId",
      displayName: "Trivia Board Id",
      type: "number",
      role: "primaryKey",
      hidden: 3,
    },
    question: {
      name: "question",
      displayName: "Question",
      type: "string",
      role: "value",
    },
    totalPoints: {
      name: "totalPoints",
      displayName: "Total Points",
      type: "number",
      role: "value",
    },
    answers: {
      name: "answers",
      displayName: "Answers",
      type: "collection",
      itemType: {
        name: "$collectionItem",
        displayName: "",
        role: "value",
        type: "object",
        get typeDef() { return (domain.types.TriviaAnswer as ObjectType) },
      },
      role: "value",
      dontSerialize: true,
    },
  },
  methods: {
  },
  dataSources: {
  },
}
export const TriviaAnswer = domain.types.TriviaAnswer = {
  name: "TriviaAnswer",
  displayName: "Trivia Answer",
  type: "object",
  props: {
    triviaAnswerId: {
      name: "triviaAnswerId",
      displayName: "Trivia Answer Id",
      type: "number",
      role: "value",
    },
    answer: {
      name: "answer",
      displayName: "Answer",
      type: "string",
      role: "value",
    },
    points: {
      name: "points",
      displayName: "Points",
      type: "number",
      role: "value",
    },
    triviaBoard: {
      name: "triviaBoard",
      displayName: "Trivia Board",
      type: "model",
      get typeDef() { return (domain.types.TriviaBoard as ModelType) },
      role: "value",
      dontSerialize: true,
    },
    position: {
      name: "position",
      displayName: "Position",
      type: "number",
      role: "value",
    },
  },
}
export const TriviaService = domain.services.TriviaService = {
  name: "TriviaService",
  displayName: "Trivia Service",
  type: "service",
  controllerRoute: "TriviaService",
  methods: {
    getRandomTriviaBoard: {
      name: "getRandomTriviaBoard",
      displayName: "Get Random Trivia Board",
      transportType: "item",
      httpMethod: "POST",
      params: {
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "model",
        get typeDef() { return (domain.types.TriviaBoard as ModelType) },
        role: "value",
      },
    },
    getRandomTriviaBoardWithNoAnswers: {
      name: "getRandomTriviaBoardWithNoAnswers",
      displayName: "Get Random Trivia Board With No Answers",
      transportType: "item",
      httpMethod: "POST",
      params: {
      },
      return: {
        name: "$return",
        displayName: "Result",
        // Type not supported natively by Coalesce - falling back to string.
        type: "string",
        role: "value",
      },
    },
    getTriviaBoardOfId: {
      name: "getTriviaBoardOfId",
      displayName: "Get Trivia Board Of Id",
      transportType: "item",
      httpMethod: "POST",
      params: {
        id: {
          name: "id",
          displayName: "Id",
          type: "number",
          role: "value",
        },
      },
      return: {
        name: "$return",
        displayName: "Result",
        type: "model",
        get typeDef() { return (domain.types.TriviaBoard as ModelType) },
        role: "value",
      },
    },
  },
}

interface AppDomain extends Domain {
  enums: {
  }
  types: {
    TriviaAnswer: typeof TriviaAnswer
    TriviaBoard: typeof TriviaBoard
  }
  services: {
    TriviaService: typeof TriviaService
  }
}

solidify(domain)

export default domain as AppDomain
