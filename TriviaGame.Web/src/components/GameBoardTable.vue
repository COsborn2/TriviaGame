<template>
  <v-simple-table>
    <tbody>
    <v-item v-for="(triviaAnswer, index) in answers" v-slot="{ active, toggle }">
      <tr>
        <template v-if="triviaAnswer">
          <td :class="'thin-right-border ' + (isRight ? 'thick-left-border' : '')" style="width: 90%"
              v-if="!active || !isHost">
            {{ triviaAnswer.answer }} {{ triviaAnswer.wonBy }}
          </td>
          <td :class="'text-center ' + (isLeft && !isHost ? 'thick-right-border' : '')" style="width: 10%"
              v-if="!active || !isHost">
            {{ triviaAnswer.points }}
          </td>
          <td :class="(isRight ? 'thick-left-border' : '')" :colspan="(shouldColspan ? 2 : 0)" style="width: 100%" v-if="active && isHost">
            <v-row class="justify-space-around" no-gutters>
              <v-col align="center" cols="4"><v-btn small @click="teamOne(triviaAnswer)" class="primary">Team One</v-btn></v-col>
              <v-col align="center" cols="4"><v-btn small @click="noTeam(triviaAnswer)" class="warning">No Team</v-btn></v-col>
              <v-col align="center" cols="4"><v-btn small @click="teamTwo(triviaAnswer)" class="secondary">Team Two</v-btn></v-col>
            </v-row>
          </td>
          <td align="right" class="px-0 thick-left-border" v-if="isHost">
            <v-btn @click="toggle" x-small style="height: 100%">
              <v-icon>fas fa-arrow-{{ active ? 'right' : 'left' }}</v-icon>
            </v-btn>
          </td>
        </template>
        <template v-else>
          <td colspan="100%" :class="($vuetify.breakpoint.lgAndUp ? (isLeft ? 'thick-right-border' : 'thick-left-border') : '') + ' unanswered'"
              align="center">
            <v-chip v-if="displayChip(index)">
              {{ chipNumber(index) }}
            </v-chip>
          </td>
        </template>
      </tr>
    </v-item>
    </tbody>
  </v-simple-table>
</template>

<script lang="ts">
import {Component, Prop, Vue} from 'vue-property-decorator';
import {TriviaAnswer} from '@/models.g';

@Component({})
export default class GameBoardTable extends Vue {
  @Prop({type: Boolean, required: true})
  public isHost!: boolean;

  @Prop({type: String, required: true})
  public boardSide!: string;

  @Prop({type: Array, required: true})
  public answers!: Array<TriviaAnswer | null>;

  @Prop({type: Number, required: true})
  public totalAnswersInBoard!: number;

  @Prop({type: Number, required: false})
  public indexOffset: number | undefined;

  public get shouldColspan(): boolean {
    return this.answers.filter((value) => !!value).length > 1;
  }

  public displayChip(index: number): boolean {
    return index + (this.indexOffset ?? 0) < this.totalAnswersInBoard;
  }

  public chipNumber(index: number): number {
    return index + 1 + (this.indexOffset ?? 0);
  }

  public get isLeft(): boolean {
    return this.boardSide.toLowerCase() === 'left' && this.$vuetify.breakpoint.lgAndUp;
  }

  public get isRight(): boolean {
    return this.boardSide.toLowerCase() === 'right' && this.$vuetify.breakpoint.lgAndUp;
  }

  public teamOne(answer: TriviaAnswer) {
    this.$emit('awardToTeamOne', answer);
  }

  public noTeam(answer: TriviaAnswer) {
    this.$emit('awardToNoTeam', answer);
  }

  public teamTwo(answer: TriviaAnswer) {
    this.$emit('awardToTeamTwo', answer);
  }
}
</script>

<style lang="scss" scoped>
.thin-right-border {
  border-right: solid 1px;
}

.thick-right-border {
  border-right: solid 3px;
}

.thick-left-border {
  border-left: solid 3px;
}

.unanswered {
  background: deepskyblue;
}
</style>
