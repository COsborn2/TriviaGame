<template>
  <v-container fluid>
    <template>
      <v-expand-transition>
        <v-card v-if="this.isInGame">
          <v-row>
            <v-col cols="12" lg="5" align="center">
              <h2>Game Id</h2>
              {{ this.currentGameSession.gameId }}
            </v-col>
            <v-col cols="1" :align="$vuetify.breakpoint.lgAndUp ? 'right' : 'center'" class="d-none d-sm-flex">
              <v-divider vertical />
            </v-col>
            <v-col cols="12" lg="4" align="center">
              <h2>Connection Id</h2>
              {{ this.connection.connectionId }}
            </v-col>
            <v-col cols="12" lg="1" align="center" align-self="center">
              <v-btn class="mr-3" color="red" @click="this.disconnect">Disconnect</v-btn>
            </v-col>
          </v-row>
        </v-card>
      </v-expand-transition>
    </template>

    <template>
      <v-card
        class="mx-auto mt-5"
        max-width="800"
        tile
        v-if="this.isInGame">
        <v-row>
          <v-col align-self="center" align="center">
            <v-btn @click="this.hostButton" v-if="!hostPresent || isCurrentHost" color="red">
              {{ hostPresent ? 'Click to leave as host' : 'Click here to host' }}
            </v-btn>
            <h1 align="center">{{ !hostPresent ? 'No Host' : currentGameSession.host.connectionId }}</h1>
          </v-col>
        </v-row>
      </v-card>
    </template>

    <template>
      <v-card
        class="mx-auto mt-5"
        max-width="800"
        tile
        v-if="this.isInGame">
        <v-row>
          <v-col align="center">
            <h2>Total Players: {{ currentGameSession.players.length }}</h2>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12" lg="6" align="center">
            <player-list @joinTeamClicked="joinTeamButtonClickedLeft"
                         :team-points-total="currentGameSession.totalPointsTeamOne"
                         :is-in-team="isInAnyTeam"
                         :players="currentGameSession.teamOnePlayers"
                         :connection-id="connection.connectionId"
                         team-name="Team One" />
          </v-col>
          <v-col cols="12" lg="6" align="center">
            <player-list @joinTeamClicked="joinTeamButtonClickedRight"
                         :team-points-total="currentGameSession.totalPointsTeamTwo"
                         :is-in-team="isInAnyTeam"
                         :players="currentGameSession.teamTwoPlayers"
                         :connection-id="connection.connectionId"
                         team-name="Team Two" />
          </v-col>
        </v-row>
      </v-card>
    </template>

    <template>
      <v-card
        class="mx-auto mt-5"
        max-width="800"
        tile
        v-if="this.isInGame">
        <h1 align="center" style="font-size: xxx-large">{{ pointsOnBoard }}</h1>
        <h2 align="center">{{ currentGameSession.triviaBoard.question }}</h2>

        <game-board @awardPointsForAnswer="awardPointsForAnswer" :is-host="isCurrentHost" :total-answers-in-board="currentGameSession.totalAnswers" :trivia-answers="triviaAnswers" />
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
import {Component, Vue} from 'vue-property-decorator';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {GameSessionInfo} from '@/GameSessionInfo';
import {TriviaAnswer} from '@/models.g';
import GameBoard from '@/components/GameBoard.vue';
import {Player, Team} from '@/Player';
import PlayerList from '@/components/PlayerList.vue';

@Component({
  components: {
    PlayerList,
    GameBoard,
  },
})
export default class Home extends Vue {

  public get currentPlayer(): Player {
    const filteredList = this.currentGameSession.players.filter(this.searchFunction);

    return filteredList[0];
  }

  public get isInAnyTeam(): boolean {
    return this.connectionIdExists(this.currentGameSession.teamOnePlayers) ||
      this.existsFunc(this.currentGameSession.teamTwoPlayers);
  }

  public get pointsOnBoard(): number {
    if (!this.triviaAnswers || this.triviaAnswers.length < 1) { return 0; }
    return this.triviaAnswers.map((value) => value.points).reduce((a, b) => (a ?? 0) + (b ?? 0)) ?? 0;
  }

  public get isCurrentHost(): boolean {
    return this.currentGameSession?.host?.connectionId === this.connection.connectionId;
  }

  public get hostPresent(): boolean {
    return !!this.currentGameSession.host;
  }

  public get triviaAnswers(): TriviaAnswer[] | null {
    return this.currentGameSession.triviaBoard.answers;
  }

  public get isInGame() {
    return this.currentGameSession.gameId;
  }
  public connection: HubConnection;
  public userInput: string = '';
  public currentGameSession!: GameSessionInfo;

  constructor() {
    super();

    this.connection = new HubConnectionBuilder()
      .withUrl('/gameboardhub')
      .build();

    this.currentGameSession = new GameSessionInfo();
  }

  public awardPointsForAnswer(answer: TriviaAnswer) {
    this.connection.invoke('AwardAnswerToTeam', answer);
  }

  public joinTeamButtonClickedLeft() {
    this.joinTeamButtonClicked(Team.One);
  }

  public joinTeamButtonClickedRight() {
    this.joinTeamButtonClicked(Team.Two);
  }

  public joinTeamButtonClicked(team: Team) {
    const player = this.currentPlayer;
    player.team = team;
    this.connection.invoke('PlayerUpdated', player);
  }

  public indexOfPlayerWithConnectionId(connId: string): number {
    for (let i = 0; i < this.currentGameSession.players.length; i++) {
      if (this.currentGameSession.players[i].connectionId === connId) {
        return i;
      }
    }
    return -1;
  }

  public connectionIdExists(arr: Player[]): boolean {
    return this.existsFunc(arr.filter(this.searchFunction));
  }

  public searchFunction: (value: Player) => boolean = (value: Player) => {
    return value.connectionId === this.connection.connectionId;
  }

  public existsFunc: (arr: Player[]) => boolean = (arr: Player[]) => {
    return arr.length === 1 && arr[0].connectionId === this.connection.connectionId;
  }

  public async hostButton() {
    if (this.isCurrentHost) {
      // leaving as host
      await this.connection.invoke('LeaveHost');
    } else {
      // joining as host
      await this.connection.invoke('HostGame');
    }
  }

  public async createGame() {
    await this.connection.invoke('CreateGame');
  }

  public joinGame() {
    this.connection.invoke('JoinGame', this.userInput);
  }

  public async disconnect() {
    await this.connection.invoke('LeaveGame');
    this.currentGameSession.gameId = null;
  }

  public async created() {
    this.connection.on('ConfirmPlayerAdded', (player: Player) => {
      const index = this.indexOfPlayerWithConnectionId(player.connectionId);

      if (index > -1) { return; }

      this.currentGameSession.players.push(player);
    });

    this.connection.on('ConfirmPlayerRemoved', (player: Player) => {
      const index = this.indexOfPlayerWithConnectionId(player.connectionId);

      if (index < 0) { return; }

      this.currentGameSession.players.splice(index, 1);

      // if player that left was the host
      if (player.connectionId === this.currentGameSession.host?.connectionId) {
        this.currentGameSession.host = null;
      }
    });

    this.connection.on('PlayerTeamUpdated', (player: Player) => {
      const index = this.indexOfPlayerWithConnectionId(player.connectionId);

      if (index < 0) { return; }

      this.currentGameSession.players[index].team = player.team;
    });

    this.connection.on('ReceiveGameInformation', (sessionInfo: GameSessionInfo) => {
      this.currentGameSession.gameId = sessionInfo.gameId;
      this.currentGameSession.players = sessionInfo.players;
      this.currentGameSession.triviaBoard = sessionInfo.triviaBoard;
      this.currentGameSession.totalAnswers = sessionInfo.totalAnswers;
      this.currentGameSession.host = sessionInfo.host;
    });

    this.connection.on('HostChanged', (host: Player) => {
      if (!host) {
        this.currentGameSession.host = null;
      }

      if (!this.currentGameSession.host) {
        this.currentGameSession.host = new Player(host.connectionId, host.team);
        return;
      }

      this.currentGameSession.host.connectionId = host.connectionId;
      this.currentGameSession.host.team = host.team;
    });

    this.connection.on('TriviaAnswersRevealed', (triviaAnswers: TriviaAnswer[]) => {
      const idToIndex: Record<number, number | undefined | null> = {};

      // in *any* case, if it exists we want to update it
      if (this.currentGameSession.triviaBoard.answers) {
        const len: number = this.currentGameSession.triviaBoard.answers?.length ?? 0;

        // search our array for given answer id
        for (let i = 0; i < len; i++) {
          const cur = this.currentGameSession.triviaBoard.answers[i];
          if (!cur.triviaAnswerId) { continue; }
          idToIndex[cur.triviaAnswerId] = i;
        }

        for (const item of triviaAnswers) {
          if (!item.triviaAnswerId) { continue; }

          const index = idToIndex[item.triviaAnswerId];
          if (index !== undefined && index !== null) {
            this.currentGameSession.triviaBoard.answers[index].wonBy = item.wonBy;
          }
        }
      }

      // Don't update array if host and already have all of the answers for the board
      if (this.isCurrentHost &&
        this.currentGameSession.triviaBoard.answers?.length ===
        this.currentGameSession.totalAnswers) {
        return;
      }

      if (this.isCurrentHost) {
        this.currentGameSession.triviaBoard.answers = triviaAnswers;
        return;
      }

      for (const cur of triviaAnswers) {
        if (!cur.triviaAnswerId) { continue; }

        // if we don't already have this id
        if (idToIndex[cur.triviaAnswerId] === undefined) {
          this.currentGameSession.triviaBoard.answers?.push(cur);
        }
      }
    });

    this.connection.start();
  }
}
</script>

<style lang="scss">
.disable-hover-white tr:hover:not(.v-table__expanded__content) {
  background: white !important;
}
</style>
