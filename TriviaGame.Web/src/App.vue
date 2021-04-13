<template>
  <v-app id="vue-app">

    <v-app-bar app color="primary" dark clipped-left>
      <v-toolbar-title>
        <router-link to="/" class="white--text" style="text-decoration: none">
          Trivia Game
        </router-link>
      </v-toolbar-title>
    </v-app-bar>

    <v-main>
      <transition
        name="router-transition"
        mode="out-in"
        @enter="routerViewOnEnter"
        appear
      >
        <!-- https://stackoverflow.com/questions/52847979/what-is-router-view-key-route-fullpath -->
        <router-view ref="routerView" :key="$route.path" />
      </transition>
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
  components: {},
})
export default class App extends Vue {
  public drawer: boolean | null = null;
  public routeComponent: Vue | null = null;

  get routeMeta() {
    if (!this.$route || this.$route.name === null) { return null; }

    return this.$route.meta;
  }

  public routerViewOnEnter() {
    this.routeComponent = this.$refs.routerView as Vue;
  }

  public created() {
    const baseTitle = document.title;
    this.$watch(
      () => (this.routeComponent as any)?.pageTitle,
      (n: string | null | undefined) => {
        if (n) {
          document.title = n + ' - ' + baseTitle;
        } else {
          document.title = baseTitle;
        }
      },
      { immediate: true },
    );
  }
}
</script>

<style lang="scss">
.router-transition-enter-active,
.router-transition-leave-active {
  // transition: 0.2s cubic-bezier(0.25, 0.8, 0.5, 1);
  transition: 0.1s ease-out;
}

.router-transition-move {
  transition: transform 0.4s;
}

.router-transition-enter,
.router-transition-leave-to {
  opacity: 0;
  // transform: translateY(5px);
}
</style>
