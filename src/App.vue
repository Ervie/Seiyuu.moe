<template>
  <v-app id="v-app" dark="">
    <v-toolbar class="toolbar" color="primary">
      <v-toolbar-title><h1>Seiyuu.Moe</h1></v-toolbar-title>
      <v-spacer></v-spacer>
    </v-toolbar>
    <router-view id="router-view" />
  </v-app>
</template>

<script>
export default {
  name: 'App'
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
