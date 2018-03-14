<template>
  <v-app id="v-app" :dark="darkMode">
    <v-toolbar class="toolbar" dense>
      <v-toolbar-title>Seiyuu Interlink</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-tooltip bottom>
        <v-btn icon v-on:click="toggleDarkMode" slot="activator">
          <v-icon color="blue darken-2">lightbulb_outline</v-icon>
        </v-btn>
        <span>Night mode</span>
      </v-tooltip>
    </v-toolbar>
    <router-view id="router-view" />
  </v-app>
</template>

<script>
export default {
  name: 'App',
  data () {
    return {
      darkMode: false
    }
  },
  methods: {
    toggleDarkMode () {
      this.darkMode = !this.darkMode
    }
  }
}
</script>

<style>
#v-app {
  font-family: "Quicksand", Helvetica, Arial, sans-serif;
  text-align: center;
  color: #5a5a5a;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}
</style>
