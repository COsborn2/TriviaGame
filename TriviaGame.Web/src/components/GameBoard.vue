<template>
  <v-item-group>
    <v-row class="ma-0">
      <v-col cols="12" lg="6" class="pa-0">
        <game-board-table
          @awardToTeamOne="awardPointsToTeamOne"
          @awardToTeamTwo="awardPointsToTeamTwo"
          @awardToNoTeam="awardPointsToNoTeam"
          :answers="firstTableRows"
          board-side="left"
          :is-host="isHost"
          :total-answers-in-board="totalAnswersInBoard" />
      </v-col>
      <v-col cols="12" lg="6" class="pa-0">
        <game-board-table
          @awardToTeamOne="awardPointsToTeamOne"
          @awardToTeamTwo="awardPointsToTeamTwo"
          @awardToNoTeam="awardPointsToNoTeam"
          :answers="secondTableRows"
          board-side="right"
          :is-host="isHost"
          :total-answers-in-board="totalAnswersInBoard"
          :index-offset="4" />
      </v-col>
    </v-row>
  </v-item-group>
</template>

<script lang="ts">
import {Component, Prop, Vue} from 'vue-property-decorator';
import {Team, TriviaAnswer} from '@/models.g';
import GameBoardTable from '@/components/GameBoardTable.vue';

@Component({
  components: {
    GameBoardTable,
  },
})
export default class GameBoard extends Vue {
  @Prop({type: Array, required: true})
  public triviaAnswers!: TriviaAnswer[] | null;

  @Prop({type: Number, required: true})
  public totalAnswersInBoard!: number;

  @Prop({type: Boolean, required: true})
  public isHost!: boolean;

  public get firstTableRows(): Array<TriviaAnswer | null> {
    return this.interpolatedTriviaAnswers.slice(0, 4);
  }

  public get secondTableRows(): Array<TriviaAnswer | null> {
    return this.interpolatedTriviaAnswers.slice(4, 8);
  }

  public awardPointsToNoTeam(answer: TriviaAnswer) {
    this.awardPointsForAnswer(answer, Team.NoTeam);
  }

  public awardPointsToTeamOne(answer: TriviaAnswer) {
    this.awardPointsForAnswer(answer, Team.One);
  }

  public awardPointsToTeamTwo(answer: TriviaAnswer) {
    this.awardPointsForAnswer(answer, Team.Two);
  }

  public awardPointsForAnswer(answer: TriviaAnswer, wonBy: Team) {
    answer.wonBy = wonBy;
    this.$emit('awardPointsForAnswer', answer);
  }

  public get interpolatedTriviaAnswers(): Array<TriviaAnswer | null> {
    const sortedRows: TriviaAnswer[] = this.triviaAnswers ?? [];
    // These should already be sorted but I'll leave this in just in case JSON messes with the order
    sortedRows.sort((a, b) => {
      const left: number = a?.position ?? 0;
      const right: number = b?.position ?? 0;
      return left - right;
    });

    const interpolatedAnswers: Array<TriviaAnswer | null> = [];
    for (let i = 0, index = 0; i < 8; i++) {
      if (index > sortedRows.length - 1) { // out of bounds
        interpolatedAnswers.push(null);
        continue;
      }

      const cur: TriviaAnswer = sortedRows[index];

      if (cur && cur?.position === i) {
        interpolatedAnswers.push(cur);
        index++;
        continue;
      }

      interpolatedAnswers.push(null);
    }

    return interpolatedAnswers;
  }
}
</script>

<style lang="scss" scoped>

</style>
