<template>
  <div>
    <v-btn @click="joinTeamButtonClicked" class="primary" v-if="!isInTeam">Join {{ teamName }}</v-btn>
    <v-expansion-panels>
      <v-expansion-panel>
        <v-expansion-panel-header>
          <v-row class="ma-0 pa-0" no-gutters>
            <v-col align="center">{{ teamName }}</v-col>
            <v-col align="center" vertical><v-divider vertical/></v-col>
            <v-col align="center">{{ players.length }}</v-col>
          </v-row>
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <v-simple-table>
            <tr v-for="player in players">
              <td>
                <v-icon v-if="player.connectionId === connectionId">fas fa-user</v-icon>
                {{ player.connectionId }}
              </td>
            </tr>
          </v-simple-table>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>
  </div>
</template>

<script lang="ts">
import {Component, Prop, Vue} from "vue-property-decorator";
import {Player} from "@/Player";

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
  
  public joinTeamButtonClicked() {
    this.$emit('joinTeamClicked')
  }
}
</script>

<style lang="scss" scoped>

</style>