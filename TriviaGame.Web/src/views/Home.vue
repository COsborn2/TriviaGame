<template>
  <v-container fluid>
    <template>
      <v-expand-transition>
        <v-card v-if="this.isInGame">
          <v-row>
            <v-col cols="5" align="center">
              <h2>Game Id</h2>
              {{ this.currentGameSession.gameId }}
            </v-col>
            <v-col cols="1" align="center">
              <v-divider vertical />
            </v-col>
            <v-col cols="4" align="center">
              <h2>Connection Id</h2>
              {{ this.connection.connectionId }}
            </v-col>
            <v-col cols="2" align="right" align-self="center">
              <v-btn class="mr-3" color="red" @click="this.disconnect">Disconnect</v-btn>
            </v-col>
          </v-row>
        </v-card>
      </v-expand-transition>
    </template>
    
    <template>
      <v-card
        class="mx-auto mt-5"
        min-width="800"
        max-width="800"
        tile
        v-if="this.isInGame">
        <v-row>
          <v-col align-self="center" align="center">
            <v-btn @click="this.hostButton" v-if="!hostPresent || isCurrentHost" color="red">
              {{ hostPresent ? 'Click to leave as host' : 'Click here to host' }}
            </v-btn>
            <h1 align="center">{{ !hostPresent ? 'No Host' : currentGameSession.hostId }}</h1>
          </v-col>
        </v-row>
      </v-card>
    </template>
    
    <template>
      <v-card
        class="mx-auto mt-5"
        min-width="800"
        max-width="800"
        tile
        v-if="this.isInGame">
        <h1 align="center" style="font-size: xxx-large">{{ pointsOnBoard }}</h1>
        <h2 align="center">{{ currentGameSession.triviaBoard.question }}</h2>
        
        <game-board :total-answers-in-board="currentGameSession.totalAnswers" :trivia-answers="triviaAnswers" />
      </v-card>
    </template>

    <template v-if="!this.isInGame">
      <v-card
        class="mx-auto mt-5"
        min-width="800"
        max-width="800"
        tile>
        <v-row justify="center">
          <v-col cols="4" align="center">
            <h1>Join Game</h1>
            <v-text-field  v-model="userInput" label="Game ID"/>
            <v-btn @click="this.joinGame" color="primary">Connect</v-btn>
          </v-col>

          <v-col cols="2" align="center">
            <v-divider vertical/>
          </v-col>

          <v-col cols="4" align="center">
            <h1>Create Game</h1>
            <v-btn @click="this.createGame">Create Game</v-btn>
          </v-col>
        </v-row>
      </v-card>
    </template>
  </v-container>
</template>

<script lang="ts">
import {Component, Vue} from "vue-property-decorator";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {GameSessionInfo} from "@/GameSessionInfo";
import {TriviaAnswer} from "@/models.g";
import GameBoard from "@/components/GameBoard.vue";

@Component({
  components: {
    GameBoard
  }
})
export default class Home extends Vue {
  public connection: HubConnection;
  public messages: string[] = [];
  public userInput: string = "";
  public currentGameSession!: GameSessionInfo;

  public get pointsOnBoard(): number {
    if (!this.triviaAnswers || this.triviaAnswers.length < 1) return 0;
    return this.triviaAnswers.map(value => value.points).reduce((a, b) => (a ?? 0) + (b ?? 0)) ?? 0;
  }
  
  public get isCurrentHost(): boolean {
    return this.currentGameSession.hostId === this.connection.connectionId;
  }
  
  public get hostPresent(): boolean {
    return !!this.currentGameSession.hostId && this.currentGameSession.hostId !== '';
  }
  
  public get triviaAnswers(): TriviaAnswer[] | null {
    return this.currentGameSession.triviaBoard.answers;
  }
  
  public get isInGame() {
    return this.currentGameSession.gameId;
  }
  
  constructor() {
    super();
    
    this.connection = new HubConnectionBuilder()
      .withUrl('/gameboardhub')
      .build();
    
    this.currentGameSession = new GameSessionInfo();
  }
  
  public async hostButton() {
    if (this.isCurrentHost) {
      // leaving as host
      await this.connection.invoke('LeaveHost')
      this.currentGameSession.triviaBoard.answers = [];
    } else {
      // joining as host
      await this.connection.invoke('HostGame')
    }
  }
  
  public async createGame() {
    await this.connection.invoke('CreateGame');
  }
  
  public joinGame() {
    this.connection.invoke('JoinGame', this.userInput).catch(err => {
      console.log(err)
    });
  }
  
  public async disconnect() {
    await this.connection.invoke('LeaveGame')
    this.currentGameSession.gameId = null;
  }
  
  public async created() {
    this.connection.on('ConfirmPlayerAdded', (userIds: string[]) => {
      this.messages = userIds;
    });
    
    this.connection.on('ConfirmPlayerRemoved', (userIds: string[]) => {
      this.messages = userIds;
    });
    
    this.connection.on('ReceiveGameInformation', (sessionInfo: GameSessionInfo) => {
      this.currentGameSession = sessionInfo;
    })
    
    this.connection.on('HostChanged', (hostId: string) => {
      this.currentGameSession.hostId = hostId;
    })
    
    this.connection.on('TriviaAnswersRevealed', (triviaAnswers: TriviaAnswer[]) => {
      for (let i = 0; i < triviaAnswers.length; i++) {
        let cur: TriviaAnswer = triviaAnswers[i];
        if (!cur) continue;
        let indexOf = this.currentGameSession.triviaBoard.answers?.indexOf(cur) ?? 0;
        if (indexOf < 0) {
          this.currentGameSession.triviaBoard.answers?.push(cur);
        }
      }
    })
    
    this.connection.start()
      .catch(err => console.log(err));
  }
}
</script>

<style lang="scss">
.disable-hover-white tr:hover:not(.v-table__expanded__content) {
  background: white !important;
}
</style>