<template>
  <div>
    <h1>Points: {{ teamPointsTotal }}</h1>
    <v-btn @click="joinTeamButtonClicked" class="primary mb-1" :color="color" v-if="!isInTeam">Join {{ teamName }}</v-btn>
    <v-expansion-panels>
      <v-expansion-panel>
        <v-expansion-panel-header :class="expansionHeaderClass + ' white--text'">
          <v-row class="ma-0 pa-0" no-gutters>
            <v-col align="center">{{ teamName }}</v-col>
            <v-col align="center" vertical><v-divider vertical/></v-col>
            <v-col align="center">{{ players.length }}</v-col>
          </v-row>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-simple-table class="mt-2">
            <tr v-for="player in players">
              <td>
                <v-icon v-if="player.connectionId === connectionId">fas fa-user</v-icon>
                <v-icon v-else-if="!!hostConnectionId && player.connectionId === hostConnectionId">fas fa-bullhorn</v-icon>
              </td>
              <td align="center" style="width: 100%">
                {{ player.connectionId }}
              </td>
              <td>
                <v-chip v-if="player.buzzerPosition && player.buzzerPosition > 0">{{ player.buzzerPosition }}</v-chip>
              </td>
            </tr>
          </v-simple-table>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script lang="ts">
import {Component, Prop, Vue} from 'vue-property-decorator';
import {Player} from '@/Player';

@Component({})
export default class PlayerList extends Vue {
  @Prop({type: Boolean, required: true})
  public isInTeam!: boolean;

  @Prop({type: String, required: true})
  public teamName!: string;

  @Prop({type: Array, required: true})
  public players!: Player[];

  @Prop({type: String, required: true})
  public connectionId!: string;

  @Prop({type: Number, required: true})
  public teamPointsTotal!: number;

  @Prop({type: String, required: true})
  public color!: string;

  @Prop({type: String, required: true})
  public expansionHeaderClass!: string;

  @Prop({required: true})
  public hostConnectionId!: string | null;

  public joinTeamButtonClicked() {
    this.$emit('joinTeamClicked');
  }
}
</script>

<style lang="scss" scoped>
</style>
