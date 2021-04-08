<template>
  <v-row class="ma-0" style="background: black">
    <v-col cols="6" class="pr-0">
      <v-simple-table>
        <tbody>
        <tr v-for="(item, index) in interpolatedTriviaAnswers.slice(0, 4)" :key="'leftTable' + index">
          <template v-if="item">
            <td class="thin-right-border">{{ item.answer }}</td>
            <td class="text-center thick-right-border" style="width: 10%">{{ item.points }}</td>
          </template>
          <template v-else>
            <td class="thin-right-border unanswered" align="center" colspan="2">
              <v-chip v-if="index < totalAnswersInBoard">{{ index + 1 }}</v-chip>
            </td>
          </template>
        </tr>
        </tbody>
      </v-simple-table>
    </v-col>
    <v-col cols="6" class="pl-0">
      <v-simple-table>
        <tbody>
        <tr v-for="(item, index) in interpolatedTriviaAnswers.slice(4, 8)" :key="'leftTable' + index">
          <template v-if="item">
            <td class="thin-right-border thick-left-border">{{ item.answer }}</td>
            <td class="text-center" style="width: 10%">{{ item.points }}</td>
          </template>
          <template v-else>
            <td class="thick-left-border unanswered" align="center" colspan="2">
              <v-chip v-if="index + 4 < totalAnswersInBoard">{{ index + 4 + 1 }}</v-chip>
            </td>
          </template>
        </tr>
        </tbody>
      </v-simple-table>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import {TriviaAnswer} from "@/models.g";

@Component({})
export default class GameBoard extends Vue {
  @Prop({type: Array, required: true})
  public triviaAnswers!: TriviaAnswer[] | null;
  
  @Prop({type: Number, required: true})
  public totalAnswersInBoard!: number;

  public get interpolatedTriviaAnswers(): (TriviaAnswer | null)[] {
    let sortedRows: TriviaAnswer[] = this.triviaAnswers ?? [];
    // These should already be sorted but I'll leave this in just in case JSON messes with the order
    sortedRows.sort((a, b) => {
      let left: number = a?.position ?? 0;
      let right: number = b?.position ?? 0;
      return left - right;
    })

    let interpolatedAnswers: (TriviaAnswer | null) [] = [];
    for (let i = 0, index = 0; i < 8; i++) {
      if (index > sortedRows.length - 1) { // out of bounds
        interpolatedAnswers.push(null)
        continue;
      }

      let cur: TriviaAnswer = sortedRows[index];

      if (cur && cur?.position === i) {
        interpolatedAnswers.push(cur);
        index++;
        continue;
      }

      interpolatedAnswers.push(null)
    }

    return interpolatedAnswers;
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