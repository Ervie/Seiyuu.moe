<template>
  <v-app id="v-app" dark>
    <toolbar></toolbar>
    <router-view id="router-view" />
  </v-app>
</template>

<script>
import Toolbar from '@/components/Toolbar.vue'

export default {
  name: 'App',
  components: {
    'toolbar': Toolbar
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
